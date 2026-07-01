using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class CreateCreditCardEndpoint : IEndpoint
{
    public async Task<IResult> CreateCreditCardAsync(
        Guid accountId,
        CreateCreditCardRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCreditCard(accountId, request.CardId, request.Label, request.Type, request.IndividualLimit);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.CreditCards}/{{accountId}}/cards", CreateCreditCardAsync)
           .WithTags(Routes.CreditCards)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Cria um cartão de crédito para a conta.");
    }
}

internal sealed record CreateCreditCardRequest(Guid CardId, string Label, CardType Type, decimal? IndividualLimit = null);
