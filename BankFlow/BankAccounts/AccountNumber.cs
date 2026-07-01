using System.Text.RegularExpressions;

namespace BankFlow;

public record AccountNumber
{
    public string Number { get; }
    public string Digit { get; }

    public AccountNumber(string number, string digit)
    {
        if (!Regex.IsMatch(number, @"^\d{6,12}$"))
            throw new ArgumentException("Invalid account number.");

        if (!Regex.IsMatch(digit, @"^\d$"))
            throw new ArgumentException("Invalid account digit.");

        Number = number;
        Digit = digit;
    }

    public static implicit operator AccountNumber(string accountNumber) => new(accountNumber.Split('-')[0], accountNumber.Split('-')[1]);

    public override string ToString() => $"{Number}-{Digit}";
}