using Microsoft.EntityFrameworkCore;

namespace BankFlow.Infrastructure.Repositories;

public class CreditCardAccountRepository(BankFlowDbContext dbContext) : Repository(dbContext), ICreditCardAccountRepository
{
    public async Task<CreditCardAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCardAccount>()
            .Include(e=>e.CreditCards)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<CreditCardAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCardAccount>().Include(e=>e.CreditCards).FirstOrDefaultAsync(a => a.CustomerId == customerId, cancellationToken);
    }

    public void Add(CreditCardAccount account)
    {
        dbContext.Add(account);
    }

    public void Update(CreditCardAccount account)
    {
        dbContext.Update(account);
    }
}
