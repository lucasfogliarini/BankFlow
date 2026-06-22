namespace BankFlow;

public class CreditCardAccount : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public decimal TotalLimit { get; private set; }
    public decimal AdjustedLimit { get; private set; }
    public decimal UsedLimit { get; private set; }
    public decimal AvailableLimit => AdjustedLimit - UsedLimit;
    public DateTime InvoiceClosingDate { get; private set; }
    public DateTime InvoiceDueDate { get; private set; }
    public IList<CreditCardTransaction> Transactions { get; private set; } = [];

    private CreditCardAccount()
    {
    }

    public static CreditCardAccount Create(Guid customerId, decimal totalLimit)
    {
        if (totalLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(totalLimit), "Total limit cannot be negative.");

        var now = DateTime.UtcNow;
        return new CreditCardAccount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TotalLimit = totalLimit,
            AdjustedLimit = totalLimit, // Inicialmente o limite ajustado é o total aprovado
            UsedLimit = 0,
            InvoiceClosingDate = new DateTime(now.Year, now.Month, 2).AddMonths(1), // Próximo dia 2
            InvoiceDueDate = new DateTime(now.Year, now.Month, 9).AddMonths(1)     // Próximo dia 9
        };
    }

    public void AdjustLimit(decimal newLimit)
    {
        if (newLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(newLimit), "Limit cannot be negative.");

        if (newLimit > TotalLimit)
            throw new InvalidOperationException($"Cannot adjust limit to {newLimit} because it exceeds the total approved limit of {TotalLimit}.");

        if (newLimit < UsedLimit)
            throw new InvalidOperationException($"Cannot reduce limit to {newLimit} because the current used limit is {UsedLimit}.");

        AdjustedLimit = newLimit;
    }

    public void ProcessTransaction(CreditCard card, decimal amount, string merchant, string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Transaction amount must be positive.");

        if (card.CreditCardAccountId != this.Id)
            throw new InvalidOperationException("Card does not belong to this account.");

        if (card.Status != CardStatus.Active)
            throw new InvalidOperationException($"Transaction failed. Card is {card.Status}.");

        if (amount > AvailableLimit)
            throw new InvalidOperationException("Transaction failed. Insufficient global account limit.");

        // Isso vai verificar e validar o limite individual do cartão interno
        card.RecordTransaction(amount);

        // Se passar, adiciona a transação e atualiza o gasto da conta
        var transaction = CreditCardTransaction.Create(this.Id, card.Id, amount, merchant, description);
        Transactions.Add(transaction);

        UsedLimit += amount;
    }
}
