using System.Text.RegularExpressions;

namespace BankFlow;

public class Bank : Entity
{
    public string Code { get; }
    public string Name { get; }

    public Bank(string code, string name)
    {
        if (!Regex.IsMatch(code, @"^\d{3}$"))
            throw new ArgumentException("Invalid bank code.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bank name is required.");

        Code = code;
        Name = name;
    }

    public override string ToString() => $"{Code} - {Name}";
}