namespace BankFlow;

public interface ICreditCardAccountRepository : IRepository
{
    Task<CreditCardAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreditCardAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    void Add(CreditCardAccount account);
    void Update(CreditCardAccount account);
}
