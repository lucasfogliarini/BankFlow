using BankFlow.Application.Accounts;
using BankFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BankFlow.WebApi.Endpoints;

internal sealed class PayBankSlipEndpoint : IEndpoint
{
    public async Task<IResult> PayBankSlipAsync(
        Guid accountId,
        PayBankSlipRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new PayBankSlip(accountId, request.Amount, request.Description);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Accounts}/{{accountId}}/pay-bankslip", PayBankSlipAsync)
           .WithTags(Routes.Accounts)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Paga um boleto bancário.");
    }
}

internal sealed record PayBankSlipRequest(decimal Amount, string? Description);
