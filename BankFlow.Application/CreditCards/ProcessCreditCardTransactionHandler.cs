namespace BankFlow.Application;

public class ProcessCreditCardTransactionHandler(
    ICreditCardAccountRepository accountRepository,
    ICreditCardRepository cardRepository)
{
    public async Task HandleAsync(ProcessCreditCardTransaction command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await cardRepository.GetByIdAsync(command.CardId, cancellationToken)
                   ?? throw new InvalidOperationException("Card not found.");

        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
                      ?? throw new InvalidOperationException("Credit card account not found.");

        account.ProcessTransaction(card, command.Amount, command.Merchant, command.Description);

        cardRepository.Update(card);
        accountRepository.Update(account);
        await accountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ProcessCreditCardTransaction(Guid AccountId, Guid CardId, decimal Amount, string Merchant, string? Description = null);
