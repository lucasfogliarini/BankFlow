namespace BankFlow;

public enum PixKeyType
{
    Cpf,
    Email,
    Phone,
    Random
}

public class PixKey
{
    public PixKeyType Type { get; }
    public string Value { get; }

    private PixKey(PixKeyType type, string value)
    {
        Type = type;
        Value = value;
    }

    public static PixKey CreateEmail(string email) => new(PixKeyType.Email, email);
    public static PixKey CreatePhone(string phone) => new(PixKeyType.Phone, phone);
    public static PixKey CreateCpf(string cpf) => new(PixKeyType.Cpf, cpf);
    public static PixKey CreateRandom() => new(PixKeyType.Random, Guid.NewGuid().ToString());
}