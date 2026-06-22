namespace BankFlow;

public class Account : AggregateRoot
{
    public required Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public required Agency Agency { get; set; }
    public required AccountNumber Number { get; set; }    
    public decimal Balance { get; private set; }
    public bool IsBlocked { get; private set; }
    public IList<AccountTransaction>? Transactions { get; private set; }
    public IList<BankAccount>? Contacts { get; private set; }
    public IList<PixKey>? PixKeys { get; private set; }

    public void Debit(TransactionType transactionType, decimal amount, string? description = null)
    {
        Withdraw(amount);
        var transaction = AccountTransaction.CreateDebit(this.Id, transactionType, amount, description);
        Transactions.Add(transaction);
    }

    public void Deposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Balance += amount;
    }
    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (IsBlocked)
            throw new InvalidOperationException("Account is blocked.");
        

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
    }
    public void AddPixKey(PixKey pixKey)
    {
        if (PixKeys.Any(x => x.Equals(pixKey)))
            throw new InvalidOperationException("Pix key already exists.");

        PixKeys.Add(pixKey);
    }
    public void AddContact(BankAccount contact)
    {
        if (Contacts.Any(x => x.Name == contact.Name))
            throw new InvalidOperationException("Contact already exists.");

        Contacts.Add(contact);
    }
}