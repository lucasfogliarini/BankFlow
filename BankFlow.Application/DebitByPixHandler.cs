namespace BankFlow.Application;

public class DebitByPixHandler(IAccountRepository accountRepository)
{
    public async Task HandleAsync(DebitByPix debitByPix, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(debitByPix);

        var account = await accountRepository.GetByIdAsync(debitByPix.AccountId, cancellationToken);
        if (account is null) return;

        account.Debit(TransactionType.Pix, debitByPix.Amount, debitByPix.Description);

        accountRepository.Update(account);
        await accountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record DebitByPix(Guid AccountId, decimal Amount, string? Description);
