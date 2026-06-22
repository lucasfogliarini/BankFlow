namespace BankFlow.Application;

public class TransferPixHandler(IAccountRepository accountRepository)
{
    public async Task HandleAsync(TransferPix transferPix, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transferPix);

        var account = await accountRepository.GetByIdAsync(transferPix.AccountId, cancellationToken);
        if (account is null) return;

        account.Debit(TransactionType.Pix, transferPix.Amount, transferPix.Description);

        accountRepository.Update(account);
        await accountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record TransferPix(Guid AccountId, decimal Amount, string? Description);
