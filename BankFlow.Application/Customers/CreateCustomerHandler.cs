namespace BankFlow.Application;

public class CreateCustomerHandler(ICustomerRepository customerRepository)
{
    public async Task HandleAsync(CreateCustomer command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var existing = await customerRepository.GetByCpfAsync(command.Cpf, cancellationToken);
        Customer customer;
        if (existing is null)
        {
            customer = Customer.Create(
                command.FullName, 
                command.Cpf, 
                command.Email, 
                command.Phone, 
                command.Address);

            customerRepository.Add(customer);
            await customerRepository.CommitScope.CommitAsync(cancellationToken);
        }
        else
        {
            customer = existing;
        }
    }
}

public record CreateCustomer(Guid CustomerId, string FullName, string Cpf, string Email, string Phone, Address Address);