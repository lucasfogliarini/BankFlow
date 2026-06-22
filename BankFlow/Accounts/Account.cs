namespace BankFlow;

public class Account : AggregateRoot
{
    private readonly List<PixKey> _pixKeys = [];
    private readonly List<Contact> _contacts = [];
    public required Agency Agency { get; set; }
    public required AccountNumber Number { get; set; }
    public required Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public decimal Balance { get; private set; }
    public bool IsBlocked { get; private set; }

    public IReadOnlyCollection<PixKey> PixKeys => _pixKeys;
    public IReadOnlyCollection<Contact> Contacts => _contacts;
}