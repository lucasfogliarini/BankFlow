namespace BankFlow.Application;

public class AdjustCreditCardLimitHandler(ICreditCardRepository cardRepository)
{
    public async Task HandleAsync(AdjustCreditCardLimit command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await cardRepository.GetByIdAsync(command.CardId, cancellationToken)
                   ?? throw new InvalidOperationException("Card not found.");

        if (command.NewLimit.HasValue && command.NewLimit.Value > card.CreditCardAccount.AdjustedLimit)
            throw new InvalidOperationException($"Card individual limit ({command.NewLimit.Value}) cannot exceed the account's adjusted limit ({card.CreditCardAccount.AdjustedLimit}).");

        card.AdjustLimit(command.NewLimit);

        cardRepository.Update(card);
        await cardRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record AdjustCreditCardLimit(Guid AccountId, Guid CardId, decimal? NewLimit);
