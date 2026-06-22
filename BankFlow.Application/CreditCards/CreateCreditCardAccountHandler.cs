namespace BankFlow.Application;

public class CreateCreditCardAccountHandler(ICreditCardAccountRepository repository)
{
    public async Task HandleAsync(CreateCreditCardAccount command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var existing = await repository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Customer already has a credit card account.");

        var account = CreditCardAccount.Create(command.CustomerId, command.TotalLimit);
        
        repository.Add(account);
        await repository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateCreditCardAccount(Guid CustomerId, decimal TotalLimit);
