using BankFlow;

public record CustomerCreated(string Cpf) : IDomainEvent;