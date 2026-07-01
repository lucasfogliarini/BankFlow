namespace BankFlow;

public interface ICustomerRepository : IRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    void Add(Customer customer);
    void Update(Customer customer);
}
