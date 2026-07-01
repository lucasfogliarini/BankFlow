using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class UnblockCreditCardEndpoint : IEndpoint
{
    public async Task<IResult> UnblockAsync(Guid accountId, Guid cardId, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new UnblockCreditCard(accountId, cardId);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.CreditCards}/{{accountId}}/cards/{{cardId}}/unblock", UnblockAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Desbloqueia um cartão de crédito.");
    }
}
