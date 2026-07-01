using System.Text.RegularExpressions;

namespace BankFlow;

public record Agency
{
    public string Value { get; private set; }

    private Agency() { }
    public Agency(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Agency is required.");

        if (!Regex.IsMatch(value, @"^\d{4}$"))
            throw new ArgumentException("Agency must contain 4 digits.");

        Value = value;
    }

    public override string ToString() => Value;
}