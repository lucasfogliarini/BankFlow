namespace BankFlow.Application;

public class AdjustCreditCardAccountLimitHandler(ICreditCardAccountRepository repository)
{
    public async Task HandleAsync(AdjustCreditCardAccountLimit command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var account = await repository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account is null)
            throw new InvalidOperationException("Credit card account not found.");

        account.AdjustLimit(command.NewLimit);

        repository.Update(account);
        await repository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record AdjustCreditCardAccountLimit(Guid AccountId, decimal NewLimit);
