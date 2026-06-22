namespace BankFlow.Application;

public class RemoveCreditCardHandler(ICreditCardRepository repository)
{
    public async Task HandleAsync(RemoveCreditCard command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = await repository.GetByIdAsync(command.CardId, cancellationToken) 
                   ?? throw new InvalidOperationException("Card not found.");
        
        repository.Remove(card);
        await repository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record RemoveCreditCard(Guid AccountId, Guid CardId);
