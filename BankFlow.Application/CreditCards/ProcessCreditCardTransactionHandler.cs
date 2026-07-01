namespace BankFlow.Application;

public class ProcessCreditCardTransactionHandler(ICreditCardRepository cardRepository)
{
    public async Task HandleAsync(ProcessCreditCardTransaction command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await cardRepository.GetByNumberAsync(command.CardNumber, cancellationToken)
                   ?? throw new InvalidOperationException("Card not found.");

        card.CreditCardAccount.ProcessTransaction(card, command.Amount, command.Merchant, command.Description);

        cardRepository.Update(card);
        await cardRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ProcessCreditCardTransaction(Guid AccountId, CardNumber CardNumber, decimal Amount, string Merchant, string? Description = null);
