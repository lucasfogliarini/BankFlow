using BankFlow.Application;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class AdjustCreditCardAccountLimitEndpoint : IEndpoint
{
    public async Task<IResult> AdjustAccountLimitAsync(Guid accountId, AdjustCreditCardAccountLimitRequest request, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new AdjustCreditCardAccountLimit(accountId, request.NewLimit);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.CreditCardAccounts}/{{accountId}}/limit", AdjustAccountLimitAsync)
            .WithTags(Routes.CreditCardAccounts)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Ajusta o limite do cartão na conta.");
    }
}

internal sealed record AdjustCreditCardAccountLimitRequest(decimal NewLimit);
