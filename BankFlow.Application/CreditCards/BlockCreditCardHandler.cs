namespace BankFlow.Application;

public class BlockCreditCardHandler(ICreditCardRepository repository)
{
    public async Task HandleAsync(BlockCreditCard command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await repository.GetByIdAsync(command.CardId, cancellationToken)
                   ?? throw new InvalidOperationException("Card not found.");

        card.Block();

        repository.Update(card);
        await repository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record BlockCreditCard(Guid AccountId, Guid CardId);
