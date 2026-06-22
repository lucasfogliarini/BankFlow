namespace BankFlow.Application.Accounts;

public class PayBankSlipHandler(IAccountRepository accountRepository)
{
    public async Task HandleAsync(PayBankSlip payBankSlip, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payBankSlip);

        var account = await accountRepository.GetByIdAsync(payBankSlip.AccountId, cancellationToken);
        if (account is null) return;

        account.Debit(TransactionType.BankSlipPayment, payBankSlip.Amount, payBankSlip.Description);

        accountRepository.Update(account);
        await accountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record PayBankSlip(Guid AccountId, decimal Amount, string? Description);
