using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class CreateCreditCardAccountEndpoint : IEndpoint
{
    public async Task<IResult> CreateAsync(CreateCreditCardAccountRequest request, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new CreateCreditCardAccount(request.CustomerId, request.TotalLimit);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.CreditCards}/accounts", CreateAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Cria uma conta de cartão de crédito para um cliente.");
    }
}

internal sealed record CreateCreditCardAccountRequest(Guid CustomerId, decimal TotalLimit);
