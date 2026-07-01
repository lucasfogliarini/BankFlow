namespace BankFlow;
public class Customer : AggregateRoot
{
    public string FullName { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;

    private Customer() { }

    public static Customer Create(string fullName, string cpf, string email, string phone, Address address)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("Cpf is required.", nameof(cpf));

        var costumer = new Customer
        {
            FullName = fullName,
            Cpf = cpf,
            Email = email ?? string.Empty,
            Phone = phone ?? string.Empty,
            Address = address
        };
        return costumer;
    }
}