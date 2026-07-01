namespace BankFlow;

public class CreditCard : Entity
{
    public Guid CreditCardAccountId { get; private set; }
    public CreditCardAccount CreditCardAccount { get; set; }
    public string Label { get; private set; } = null!;
    public CardNumber CardNumber { get; private set; } = null!;
    public CardType Type { get; private set; }
    public CardStatus Status { get; private set; }
    public decimal? Limit { get; private set; }
    public decimal UsedLimit { get; private set; }

    private CreditCard()
    {
    }
    public static CreditCard Create(string label, CardType type, decimal? limit = null, CardStatus status = CardStatus.Active)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Label cannot be empty.", nameof(label));

        if (limit.HasValue && limit.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be negative.");

        return new CreditCard
        {
            Id = Guid.NewGuid(),
            Label = label,
            CardNumber = GenerateCardNumber(),
            Type = type,
            Status = status,
            Limit = limit,
            UsedLimit = 0
        };
    }
    public void Block()
    {
        Status = CardStatus.Blocked;
    }
    public void Unblock()
    {
        Status = CardStatus.Active;
    }
    public void Approve(decimal? limit)
    {
        if (limit.HasValue && limit.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(limit));

        Limit = limit;
        Status = CardStatus.Active;
    }
    public void Reject()
    {
        Status = CardStatus.Rejected;
    }
    public void AdjustLimit(decimal? newLimit)
    {
        if (newLimit.HasValue && newLimit.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(newLimit), "Limit cannot be negative.");

        if (newLimit.HasValue && newLimit.Value < UsedLimit)
            throw new InvalidOperationException("Limit cannot be smaller than current used limit.");

        Limit = newLimit;
    }
    public void RecordTransaction(decimal amount)
    {
        if (Status != CardStatus.Active)
            throw new InvalidOperationException("Card is not active.");

        if (Limit.HasValue)
        {
            if (UsedLimit + amount > Limit.Value)
                throw new InvalidOperationException("Card individual limit exceeded.");
        }

        UsedLimit += amount;
    }
    private static string GenerateCardNumber()
    {
        var random = new Random();
        var prefix = random.Next(51, 56).ToString(); // Mastercard prefixes 51-55
        var digits = new List<int>();
        foreach (char c in prefix)
        {
            digits.Add(c - '0');
        }

        for (int i = 0; i < 13; i++)
        {
            digits.Add(random.Next(0, 10));
        }

        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            int digit = digits[i];
            if (i % 2 == 0) // Even index positions doubled (0, 2, 4...)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        digits.Add(checkDigit);

        var numberStr = string.Join("", digits);
        return $"{numberStr[0..4]} {numberStr[4..8]} {numberStr[8..12]} {numberStr[12..16]}";
    }
}
