namespace BankFlow.Application;

public class CreateCreditCardHandler(
    ICreditCardAccountRepository accountRepository,
    ICreditCardRepository cardRepository)
{
    public async Task HandleAsync(CreateCreditCard command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
                      ?? throw new InvalidOperationException("Credit card account not found.");

        if (command.Type == CardType.Physical)
        {
            var hasPhysical = await cardRepository.HasActivePhysicalCardAsync(command.AccountId, cancellationToken);
            if (hasPhysical)
                throw new InvalidOperationException("An account can only have one active physical card.");
        }

        var labelExists = await cardRepository.CardExistsAsync(command.CardId, command.Label, cancellationToken);
        if (labelExists)
            throw new InvalidOperationException("A card with this ID or label already exists.");

        if (command.IndividualLimit.HasValue)
        {
            if (command.IndividualLimit.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(command.IndividualLimit), "Individual card limit cannot be negative.");
            if (command.IndividualLimit.Value > account.AdjustedLimit)
                throw new InvalidOperationException($"Card individual limit ({command.IndividualLimit.Value}) cannot exceed the account's adjusted limit ({account.AdjustedLimit}).");
        }

        var card = CreditCard.Create(command.AccountId, command.Label, command.Type, command.IndividualLimit);
        if (command.CardId != Guid.Empty)
        {
            card.Id = command.CardId;
        }

        cardRepository.Add(card);
        await cardRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateCreditCard(Guid AccountId, Guid CardId, string Label, CardType Type, decimal? IndividualLimit = null);
