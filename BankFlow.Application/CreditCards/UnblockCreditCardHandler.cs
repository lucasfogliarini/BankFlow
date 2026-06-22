namespace BankFlow.Application;

public class UnblockCreditCardHandler(ICreditCardRepository creditCardRepository)
{
    public async Task HandleAsync(UnblockCreditCard command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await creditCardRepository.GetByIdAsync(command.CardId, cancellationToken)
                   ?? throw new InvalidOperationException("Card not found.");

        card.Unblock();

        creditCardRepository.Update(card);
        await creditCardRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record UnblockCreditCard(Guid AccountId, Guid CardId);
