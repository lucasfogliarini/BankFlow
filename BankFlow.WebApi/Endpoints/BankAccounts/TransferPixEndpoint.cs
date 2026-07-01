using BankFlow.Application.BankAccounts;
using Wolverine;

namespace BankFlow.WebApi.Endpoints;

internal sealed class TransferPixEndpoint : IEndpoint
{
    public async Task<IResult> TransferPixAsync(
        Guid accountId,
        TransferPixRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new TransferPix(accountId, request.Amount, request.Description);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Accounts}/{{accountId}}/pix/transfer", TransferPixAsync)
           .WithTags(Routes.Accounts)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Tranferência via Pix.");
    }
}

internal sealed record TransferPixRequest(decimal Amount, string? Description);
