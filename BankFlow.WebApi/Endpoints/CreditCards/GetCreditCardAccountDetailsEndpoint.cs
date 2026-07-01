using BankFlow.Application;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class GetCreditCardAccountDetailsEndpoint : IEndpoint
{
    public async Task<IResult> GetAsync(Guid accountId, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var query = new GetCreditCardAccountDetails(accountId);
        var response = await bus.InvokeAsync<CreditCardAccountDetailsResponse?>(query, cancellationToken);
        if (response is null) return Results.NotFound();
        return Results.Ok(response);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.CreditCardAccounts}/{{accountId}}/details", GetAsync)
            .WithTags(Routes.CreditCardAccounts)
            .Produces<CreditCardAccountDetailsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Obtém detalhes da conta de cartão de crédito.");
    }
}
