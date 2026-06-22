namespace BankFlow;

public class CreditCardTransaction : Entity
{
    public Guid CreditCardAccountId { get; private set; }
    public Guid CardId { get; private set; }
    public decimal Amount { get; private set; }
    public string Merchant { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CreditCardTransaction()
    {
    }

    public static CreditCardTransaction Create(Guid accountId, Guid cardId, decimal amount, string merchant, string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Transaction amount must be positive.");

        if (string.IsNullOrWhiteSpace(merchant))
            throw new ArgumentException("Merchant cannot be empty.", nameof(merchant));

        return new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardAccountId = accountId,
            CardId = cardId,
            Amount = amount,
            Merchant = merchant,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }
}
