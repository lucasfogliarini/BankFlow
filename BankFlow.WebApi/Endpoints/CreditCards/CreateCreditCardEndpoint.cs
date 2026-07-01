using BankFlow.Application;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class CreateCreditCardEndpoint : IEndpoint
{
    public async Task<IResult> CreateCreditCardAsync(
        Guid accountId,
        CreateCreditCardRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCreditCard(accountId, request.Label, request.Type, request.Limit);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.CreditCardAccounts}/{{accountId}}/cards", CreateCreditCardAsync)
           .WithTags(Routes.CreditCardAccounts)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Cria um cartão de crédito para a conta.");
    }
}

internal sealed record CreateCreditCardRequest(string Label, CardType Type, decimal? Limit = null);
