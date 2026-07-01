namespace BankFlow;

public class BankAccount : Entity
{
    public string Name { get; private set; }
    public Guid BankId { get; private set; }
    public Bank Bank { get; private set; }
    public Agency Agency { get; private set; }
    public AccountNumber Number { get; private set; }
    public PixKey? PixKey { get; private set; }

    public BankAccount(
        string name,
        Bank bank,
        Agency agency,
        AccountNumber accountNumber,
        PixKey? pixKey)
    {
        Name = name;
        Bank = bank;
        Agency = agency;
        Number = accountNumber;
        PixKey = pixKey;
    }
}