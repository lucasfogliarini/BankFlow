using Microsoft.EntityFrameworkCore;

namespace BankFlow.Infrastructure.Repositories;

public class CreditCardRepository(BankFlowDbContext dbContext) : Repository(dbContext), ICreditCardRepository
{
    public async Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCard>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CreditCard?> GetByNumberAsync(CardNumber number, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCard>().FirstOrDefaultAsync(c => c.CardNumber == number.ToString(), cancellationToken);
    }

    public async Task<List<CreditCard>> GetCardsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCard>().Where(c => c.CreditCardAccountId == accountId).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActivePhysicalCardAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCard>().AnyAsync(c => c.CreditCardAccountId == accountId && c.Type == CardType.Physical && c.Status == CardStatus.Active, cancellationToken);
    }

    public async Task<bool> CardExistsAsync(Guid cardId, string label, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<CreditCard>().AnyAsync(c => c.Id == cardId || c.Label == label, cancellationToken);
    }

    public void Add(CreditCard card)
    {
        dbContext.Add(card);
    }

    public void Update(CreditCard card)
    {
        dbContext.Update(card);
    }

    public void Remove(CreditCard card)
    {
        dbContext.Remove(card);
    }
}
