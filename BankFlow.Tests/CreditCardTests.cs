using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BankFlow.Application;

namespace BankFlow.Tests;

public class CreditCardTests
{
    [Fact]
    public void CreateAccount_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var totalLimit = 5000m;

        // Act
        var account = CreditCardAccount.Create(customerId, totalLimit);

        // Assert
        Assert.Equal(customerId, account.CustomerId);
        Assert.Equal(totalLimit, account.TotalLimit);
        Assert.Equal(totalLimit, account.AdjustedLimit);
        Assert.Equal(0m, account.UsedLimit);
        Assert.Equal(totalLimit, account.AvailableLimit);
        Assert.True(account.InvoiceClosingDate > DateTime.UtcNow);
        Assert.True(account.InvoiceDueDate > account.InvoiceClosingDate);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void AdjustLimit_WithValidValue_ShouldUpdateAdjustedLimit()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);

        // Act
        account.AdjustLimit(3000m);

        // Assert
        Assert.Equal(3000m, account.AdjustedLimit);
        Assert.Equal(3000m, account.AvailableLimit);
    }

    [Fact]
    public void AdjustLimit_ExceedingTotalLimit_ShouldThrowException()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.AdjustLimit(6000m));
        Assert.Contains("exceeds the total approved limit", exception.Message);
    }

    [Fact]
    public void AdjustLimit_BelowUsedLimit_ShouldThrowException()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);
        var card = CreditCard.Create(account.Id, "Virtual Card", CardType.Virtual);
        account.ProcessTransaction(card, 2000m, "Store");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => account.AdjustLimit(1500m));
        Assert.Contains("smaller than current used limit", exception.Message);
    }

    [Fact]
    public void CreateCard_ShouldCreateCorrectly()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var label = "Netflix Card";
        var limit = 500m;

        // Act
        var card = CreditCard.Create(accountId, label, CardType.Virtual, limit);

        // Assert
        Assert.Equal(accountId, card.CreditCardAccountId);
        Assert.Equal(label, card.Label);
        Assert.Equal(CardType.Virtual, card.Type);
        Assert.Equal(limit, card.Limit);
        Assert.Equal(CardStatus.Active, card.Status);
        Assert.Matches(@"^5[1-5]\d{2} \d{4} \d{4} \d{4}$", card.CardNumber);
    }

    [Fact]
    public void BlockAndUnblockCard_ShouldUpdateStatus()
    {
        // Arrange
        var card = CreditCard.Create(Guid.NewGuid(), "Virtual Card", CardType.Virtual);

        // Act - Block
        card.Block();
        Assert.Equal(CardStatus.Blocked, card.Status);

        // Act - Unblock
        card.Unblock();
        Assert.Equal(CardStatus.Active, card.Status);
    }

    [Fact]
    public void AdjustCardLimit_ShouldUpdateLimit()
    {
        // Arrange
        var card = CreditCard.Create(Guid.NewGuid(), "Virtual Card", CardType.Virtual, 1000m);

        // Act
        card.AdjustLimit(1500m);

        // Assert
        Assert.Equal(1500m, card.Limit);
    }

    [Fact]
    public void ProcessTransaction_WithinLimits_ShouldSucceed()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);
        var card = CreditCard.Create(account.Id, "Virtual Card", CardType.Virtual, 1000m);

        // Act
        account.ProcessTransaction(card, 400m, "Netflix", "Monthly subscription");

        // Assert
        Assert.Equal(400m, account.UsedLimit);
        Assert.Equal(4600m, account.AvailableLimit);
        Assert.Equal(400m, card.UsedLimit);
        Assert.Single(account.Transactions);

        var tx = account.Transactions.First();
        Assert.Equal(card.Id, tx.CardId);
        Assert.Equal(400m, tx.Amount);
        Assert.Equal("Netflix", tx.Merchant);
        Assert.Equal("Monthly subscription", tx.Description);
    }

    [Fact]
    public void ProcessTransaction_ExceedingIndividualCardLimit_ShouldThrowException()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);
        var card = CreditCard.Create(account.Id, "Virtual Card", CardType.Virtual, 500m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            account.ProcessTransaction(card, 600m, "Electronics Store"));
        
        Assert.Contains("individual limit exceeded", exception.Message);
    }

    [Fact]
    public void ProcessTransaction_ExceedingGlobalAccountLimit_ShouldThrowException()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);
        account.AdjustLimit(2000m);
        var card = CreditCard.Create(account.Id, "Virtual Card", CardType.Virtual); // Sem limite individual, consome o global

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            account.ProcessTransaction(card, 2500m, "Travel Agency"));
        
        Assert.Contains("Insufficient global account limit", exception.Message);
    }

    [Fact]
    public void ProcessTransaction_OnNonActiveCard_ShouldThrowException()
    {
        // Arrange
        var account = CreditCardAccount.Create(Guid.NewGuid(), 5000m);
        var card = CreditCard.Create(account.Id, "Virtual Card", CardType.Virtual);
        card.Block();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            account.ProcessTransaction(card, 100m, "Store"));
        
        Assert.Contains("Card is Blocked", exception.Message);
    }

    [Fact]
    public async Task RemoveCreditCardHandler_ShouldCallRemoveOnRepository()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var card = CreditCard.Create(accountId, "My Card", CardType.Virtual);
        var cardRepo = new FakeCreditCardRepository(new List<CreditCard> { card });
        var handler = new RemoveCreditCardHandler(cardRepo);

        // Act
        await handler.HandleAsync(new RemoveCreditCard(accountId, card.Id));

        // Assert
        var cards = await cardRepo.GetCardsByAccountIdAsync(accountId);
        Assert.Empty(cards);
    }

    [Fact]
    public async Task GetCreditCardAccountDetails_ShouldReturnCorrectData()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var account = CreditCardAccount.Create(customerId, 10000m);
        var card = CreditCard.Create(account.Id, "Main Card", CardType.Physical);
        account.ProcessTransaction(card, 2500m, "Amazon");
        account.ProcessTransaction(card, 150m, "Spotify");

        var accountRepo = new FakeCreditCardAccountRepository(account);
        var cardRepo = new FakeCreditCardRepository(new List<CreditCard> { card });
        var handler = new GetCreditCardAccountDetailsHandler(accountRepo, cardRepo);

        // Act
        var result = await handler.HandleAsync(new GetCreditCardAccountDetails(account.Id));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.AccountId);
        Assert.Equal(card.CardNumber, result.MainCardNumber);
        Assert.Equal(7500m, result.AvailableLimit);
        Assert.Equal(2650m, result.CurrentInvoiceAmount);
        Assert.Equal(account.InvoiceClosingDate, result.InvoiceClosingDate);
        Assert.Equal(account.InvoiceDueDate, result.InvoiceDueDate);
        Assert.Equal(2, result.Transactions.Count);
        Assert.Equal("Spotify", result.Transactions[0].Merchant);
        Assert.Equal("Amazon", result.Transactions[1].Merchant);
    }

    private class FakeCreditCardAccountRepository : ICreditCardAccountRepository
    {
        private readonly CreditCardAccount _account;

        public FakeCreditCardAccountRepository(CreditCardAccount account)
        {
            _account = account;
        }

        public Task<CreditCardAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<CreditCardAccount?>(_account.Id == id ? _account : null);
        }

        public Task<CreditCardAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<CreditCardAccount?>(_account.CustomerId == customerId ? _account : null);
        }

        public void Add(CreditCardAccount account) { }
        public void Update(CreditCardAccount account) { }

        public ICommitScope CommitScope => new FakeCommitScope();
    }

    private class FakeCreditCardRepository : ICreditCardRepository
    {
        private readonly List<CreditCard> _cards = new();

        public FakeCreditCardRepository(List<CreditCard>? cards = null)
        {
            if (cards != null) _cards.AddRange(cards);
        }

        public Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_cards.FirstOrDefault(c => c.Id == id));
        }

        public Task<List<CreditCard>> GetCardsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_cards.Where(c => c.CreditCardAccountId == accountId).ToList());
        }

        public Task<bool> HasActivePhysicalCardAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_cards.Any(c => c.CreditCardAccountId == accountId && c.Type == CardType.Physical));
        }

        public Task<bool> CardExistsAsync(Guid cardId, string label, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_cards.Any(c => c.Id == cardId || c.Label.Equals(label, StringComparison.OrdinalIgnoreCase)));
        }

        public void Add(CreditCard card)
        {
            _cards.Add(card);
        }

        public void Update(CreditCard card)
        {
            var existing = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (existing != null)
            {
                _cards.Remove(existing);
                _cards.Add(card);
            }
        }

        public void Remove(CreditCard card)
        {
            var existing = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (existing != null) _cards.Remove(existing);
        }

        public ICommitScope CommitScope => new FakeCommitScope();
    }

    private class FakeCommitScope : ICommitScope
    {
        public Task<int> CommitAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
        public int Commit() => 1;
    }
}
