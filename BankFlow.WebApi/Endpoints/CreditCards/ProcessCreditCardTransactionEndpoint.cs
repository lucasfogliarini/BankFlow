using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class ProcessCreditCardTransactionEndpoint : IEndpoint
{
    public async Task<IResult> ProcessTransactionAsync(
        Guid accountId,
        ProcessCreditCardTransactionRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new ProcessCreditCardTransaction(accountId, request.CardNumber, request.Amount, request.Merchant, request.Description);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.CreditCards}/{{accountId}}/transactions", ProcessTransactionAsync)
           .WithTags(Routes.CreditCards)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Registra uma transação no cartão de crédito.");
    }
}

internal sealed record ProcessCreditCardTransactionRequest(string CardNumber, decimal Amount, string Merchant, string? Description = null);
