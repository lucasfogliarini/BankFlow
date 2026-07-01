using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class CreateCustomerEndpoint : IEndpoint
{
    public async Task<IResult> CreateAsync(CreateCustomerRequest request, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new CreateCustomer(request.CustomerId, request.FullName, request.Cpf, request.Email, request.Phone, request.Address);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost(Routes.Customers, CreateAsync)
            .WithTags(Routes.Customers)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Creates a new customer and triggers downstream requests for credit account and card.");
    }
}

internal sealed record CreateCustomerRequest(Guid CustomerId, string FullName, string Cpf, string Email, string Phone, Address Address);
