using Microsoft.EntityFrameworkCore;

namespace BankFlow.Infrastructure.Repositories;

public class AccountRepository(BankFlowDbContext dbContext) : Repository(dbContext), IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Account>()
            .Include(a => a.Transactions!)
            .Include("PixKeys")
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public void Add(Account account)
    {
        dbContext.Add(account);
    }

    public void Update(Account account)
    {
        dbContext.Update(account);
    }
}
