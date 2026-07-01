using BankFlow;
using BankFlow.Infrastructure;
using BankFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Wolverine;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDbContext();        
        builder.Services.AddRepositories();
        builder.AddOpenTelemetryExporter();
        builder.AddRateLimiter();
    }
    public static void ConfigureMessageBus(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure)
    {
        builder.UseWolverine(opts =>
        {
            //opts.Policies
            //    .OnException<Exception>()
            //    .RetryWithCooldown(
            //        TimeSpan.FromSeconds(1),
            //        TimeSpan.FromSeconds(2),
            //        TimeSpan.FromSeconds(3))
            //    .Then
            //    .PublishToDeadLetterTopic();

            //var kafkaEndpoint = builder.Configuration.GetConnectionString("KafkaServer");
            //opts.UseKafka(kafkaEndpoint).AutoProvision();

            configure?.Invoke(opts);

            opts.UseRuntimeCompilation();
            opts.CodeGeneration.AlwaysUseServiceLocationFor<BankFlowDbContext>();
        });
    }
    public static void MapHealthChecks(this WebApplication app)
    {
        var serviceInfo = ServiceInfo.Get();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("live")
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    serviceInfo.Name,
                    serviceInfo.Version,
                    app.Environment.EnvironmentName,
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                    })
                });

                await context.Response.WriteAsync(result);
            }
        });
    }
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICreditCardRepository, CreditCardRepository>();
        services.AddScoped<ICreditCardAccountRepository, CreditCardAccountRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
    }
    public static async Task MigrateAndSeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BankFlowDbContext>();
        db.Database.EnsureCreated();

        if (!await db.Set<Customer>().AnyAsync())
        {
            var address = new Address("Rua Sapé", "1020", "Porto Alegre", "RS", "91350-050");
            var customer = Customer.Create("Lucas Fogliarin Pedroso", "02277982016", "lucasfogliarini@gmail.com", "51992364249", address);
            db.Add(customer);

            var account = Account.Create(customer, "123456-7");
            db.Add(account);

            var creditCardAccount = CreditCardAccount.Create(customer.Id, CreditCardAccountStatus.Active);

            db.Add(creditCardAccount);

            creditCardAccount.AddCreditCard("Cartão Físico", CardType.Physical, 5000m);

            Console.WriteLine(creditCardAccount.Id);
            Console.WriteLine(account.Id);

            await db.CommitAsync();
        }
    }
    private static void AddDbContext(this IHostApplicationBuilder builder, string connectionStringKey = "BankFlow")
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringKey);

        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        builder.Services.AddSingleton(connection);

        void BuilderOptions(DbContextOptionsBuilder options)
        {
            if (connectionString is not null)
                options.UseNpgsql(connectionString);
            else
                options.UseSqlite(connection);

            // Use the following options only during development or troubleshooting
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }

        builder.Services.AddDbContext<BankFlowDbContext>(BuilderOptions);
        builder.Services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck<BankFlowDbContext>>(nameof(BankFlowDbContext));
    }
    private static void AddOpenTelemetryExporter(this IHostApplicationBuilder builder)
    {
        var serviceInfo = ServiceInfo.Get();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceInfo.Name, null, serviceInfo.Version))
            .WithTracing(tracerBuilder =>
            {
                tracerBuilder
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(meterBuilder =>
            {
                meterBuilder
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter();
            })
            .WithLogging(loggingBuilder =>
            {
                loggingBuilder
                    .AddOtlpExporter();
            });

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
        });
    }
    private static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("per-user", context =>
            {
                var key = context.User.FindFirstValue("sid") ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: key,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        TokensPerPeriod = 50,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(30)
                    });
            });
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Limite atingido, tente novamente em breve.", token);
            };
        });
    }
}
