using System.Text.RegularExpressions;

namespace BankFlow;

public record CardNumber
{
    public string Value { get; }

    public CardNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Card number cannot be empty.");

        // Remove espaços para validação
        var cleanValue = value.Replace(" ", "");

        if (!Regex.IsMatch(cleanValue, @"^\d{16}$"))
            throw new ArgumentException("Card number must be exactly 16 digits.");

        if (!ValidateLuhn(cleanValue))
            throw new ArgumentException("Invalid card number (Luhn validation failed).");

        // Armazena formatado com espaços
        Value = $"{cleanValue[0..4]} {cleanValue[4..8]} {cleanValue[8..12]} {cleanValue[12..16]}";
    }

    // Conversão implícita de CardNumber para string
    public static implicit operator string(CardNumber cardNumber) => cardNumber.Value;

    // Conversão implícita de string para CardNumber
    public static implicit operator CardNumber(string value) => new(value);

    public override string ToString() => Value;

    private static bool ValidateLuhn(string number)
    {
        int sum = 0;
        for (int i = 0; i < 16; i++)
        {
            int digit = number[i] - '0';
            if (i % 2 == 0) // Posições pares duplicadas (0, 2, 4...)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }
        return sum % 10 == 0;
    }
}
