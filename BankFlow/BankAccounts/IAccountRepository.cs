namespace BankFlow;

public interface IAccountRepository : IRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Account account);
    void Update(Account account);
}
