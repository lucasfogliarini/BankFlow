using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

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
        return app.MapPut($"{Routes.CreditCards}/{{accountId}}/limit", AdjustAccountLimitAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Ajusta o limite do cartão na conta.");
    }
}

internal sealed record AdjustCreditCardAccountLimitRequest(decimal NewLimit);
