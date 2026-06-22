namespace BankFlow;

public class Contact : Entity
{
    public string Name { get; private set; }
    public Bank Bank { get; private set; }
    public Agency Agency { get; private set; }
    public AccountNumber AccountNumber { get; private set; }
    public PixKey? PixKey { get; private set; }

    public Contact(
        string name,
        Bank bank,
        Agency agency,
        AccountNumber accountNumber,
        PixKey? pixKey)
    {
        Name = name;
        Bank = bank;
        Agency = agency;
        AccountNumber = accountNumber;
        PixKey = pixKey;
    }
}