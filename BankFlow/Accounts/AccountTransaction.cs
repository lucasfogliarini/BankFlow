namespace BankFlow;

public enum TransactionType
{
    Pix,
    CreditCardBillPayment,
    BankSlipPayment,    
    Reversal
}

public enum TransactionDirection
{
    Credit,
    Debit
}

public class AccountTransaction : Entity
{
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionDirection Direction { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AccountTransaction()
    {
    }

    public static AccountTransaction CreateDebit(
                Guid accountId,
                TransactionType type,
                decimal amount,
                string? description = null)
    {
        return new AccountTransaction
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Type = type,
            Direction = TransactionDirection.Debit,
            Amount = amount,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static AccountTransaction CreateCredit(
                Guid accountId,
                TransactionType type,
                decimal amount,
                string? description = null)
    {
        return new AccountTransaction
        {
            Direction = TransactionDirection.Credit,
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Type = type,            
            Amount = amount,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }
}