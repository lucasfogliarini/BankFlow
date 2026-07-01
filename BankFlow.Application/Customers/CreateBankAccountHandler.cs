namespace BankFlow.Application;

public class CreateBankAccountHandler(ICustomerRepository customerRepository, IAccountRepository accountRepository) : IDomainEventHandler<CustomerCreated>
{
    public async Task HandleAsync(CustomerCreated domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var customer = await customerRepository.GetByCpfAsync(domainEvent.Cpf, cancellationToken);
        if (customer is null)
            return;

        var account = Account.Create(customer);
        accountRepository.Add(account);
        await accountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
