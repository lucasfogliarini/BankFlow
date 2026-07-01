namespace BankFlow;

public interface IAccountRepository : IRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Account account);
}
