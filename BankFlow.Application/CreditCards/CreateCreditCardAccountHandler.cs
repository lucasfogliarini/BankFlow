namespace BankFlow.Application;

public class CreateCreditCardAccountHandler(ICreditCardAccountRepository cardRepository)
{
    public async Task HandleAsync(CreateCreditCardAccount command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var existing = await cardRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Customer already has a credit card account.");

        var account = CreditCardAccount.Create(command.CustomerId, command.TotalLimit);
        
        cardRepository.Add(account);
        await cardRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateCreditCardAccount(Guid CustomerId, decimal TotalLimit);
