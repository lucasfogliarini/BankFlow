using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class BlockCreditCardEndpoint : IEndpoint
{
    public async Task<IResult> BlockAsync(Guid accountId, Guid cardId, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new BlockCreditCard(accountId, cardId);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.CreditCards}/{{accountId}}/cards/{{cardId}}/block", BlockAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Bloqueia um cartão de crédito.");
    }
}
