var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();

var app = builder.Build();

await app.MigrateAndSeedAsync();

app.UseApplication();

app.Run();
