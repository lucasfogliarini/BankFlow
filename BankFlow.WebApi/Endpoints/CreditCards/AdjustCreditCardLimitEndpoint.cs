using BankFlow.Application;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class AdjustCreditCardLimitEndpoint : IEndpoint
{
    public async Task<IResult> AdjustAsync(Guid accountId, AdjustCreditCardLimitRequest request, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new AdjustCreditCardLimit(accountId, request.CardId, request.NewLimit);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.CreditCards}/{{cardId}}/limit", AdjustAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Ajusta o limite individual de um cartão.");
    }
}

internal sealed record AdjustCreditCardLimitRequest(Guid CardId, decimal? NewLimit);
