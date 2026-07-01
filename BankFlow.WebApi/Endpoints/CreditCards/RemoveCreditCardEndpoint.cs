using BankFlow.Application;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class RemoveCreditCardEndpoint : IEndpoint
{
    public async Task<IResult> RemoveAsync(Guid accountId, Guid cardId, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        var command = new RemoveCreditCard(accountId, cardId);
        await bus.InvokeAsync(command, cancellationToken);
        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete($"{Routes.CreditCards}/{{cardId}}", RemoveAsync)
            .WithTags(Routes.CreditCards)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Remove um cartão de crédito.");
    }
}
