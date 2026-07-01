namespace BankFlow.Application;

public class GetCreditCardAccountDetailsHandler(
    ICreditCardAccountRepository accountRepository,
    ICreditCardRepository cardRepository)
{
    public async Task<CreditCardAccountDetailsResponse?> HandleAsync(GetCreditCardAccountDetails query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var account = await accountRepository.GetByIdAsync(query.AccountId, cancellationToken);
        if (account is null)
            return null;

        var cards = await cardRepository.GetCardsByAccountIdAsync(query.AccountId, cancellationToken);
        var mainCard = cards.FirstOrDefault(c => c.Type == CardType.Physical) 
                       ?? cards.FirstOrDefault();

        var mainCardNumber = mainCard?.CardNumber ?? "No active card";

        var transactionsResponse = account.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new CreditCardTransactionResponse(
                t.Id,
                t.Merchant,
                t.Amount,
                t.CreatedAt,
                t.Description
            ))
            .ToList();

        return new CreditCardAccountDetailsResponse(
            account.Id,
            mainCardNumber,
            account.AvailableLimit,
            account.UsedLimit,
            account.InvoiceClosingDate,
            account.InvoiceDueDate,
            transactionsResponse
        );
    }
}

public record GetCreditCardAccountDetails(Guid AccountId);

public record CreditCardAccountDetailsResponse(
    Guid AccountId,
    string MainCardNumber,
    decimal AvailableLimit,
    decimal CurrentInvoiceAmount,
    DateTime InvoiceClosingDate,
    DateTime InvoiceDueDate,
    List<CreditCardTransactionResponse> Transactions
);

public record CreditCardTransactionResponse(
    Guid Id,
    string Merchant,
    decimal Amount,
    DateTime CreatedAt,
    string? Description
);