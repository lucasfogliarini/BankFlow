namespace BankFlow;

public class Account : AggregateRoot
{
    public required Guid CustomerId { get; set; }
    public required Customer Customer { get; set; }
    public required Agency Agency { get; set; }
    public required AccountNumber Number { get; set; }    
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }
    public IList<AccountTransaction>? Transactions { get; private set; }
    public IList<BankAccount>? Contacts { get; private set; }
    public IList<PixKey>? PixKeys { get; private set; }

    private Account() { }

    public static Account Create(Customer customer, AccountNumber accountNumber, Agency? agency = null)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(accountNumber);

        var account = new Account
        {
            CustomerId = customer.Id,
            Customer = customer,
            Agency = agency ?? new Agency("0001"),
            Number = accountNumber,
            Balance = 0m,
            Status = AccountStatus.Active,
            PixKeys = [PixKey.CreateRandom()]
        };

        return account;
    }

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

        if (Status == AccountStatus.Blocked)
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