namespace BankFlow.Application;

public class CreateCreditCardHandler(ICreditCardAccountRepository creditCardAccountRepository)
{
    public async Task HandleAsync(CreateCreditCard createCreditCard, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createCreditCard);

        var creditCardAccount = await creditCardAccountRepository.GetByIdAsync(createCreditCard.Id, cancellationToken)
                      ?? throw new InvalidOperationException("Credit card account not found.");

        creditCardAccount.AddCreditCard(createCreditCard.Label, createCreditCard.Type, createCreditCard.Limit);
        await creditCardAccountRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateCreditCard(Guid Id, string Label, CardType Type, decimal? Limit = null);
