namespace BankFlow;

public interface ICreditCardRepository : IRepository
{
    Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreditCard?> GetByNumberAsync(CardNumber number, CancellationToken cancellationToken = default);
    Task<List<CreditCard>> GetCardsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<bool> HasActivePhysicalCardAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<bool> CardExistsAsync(Guid cardId, string label, CancellationToken cancellationToken = default);
    void Add(CreditCard card);
    void Update(CreditCard card);
    void Remove(CreditCard card);
}
