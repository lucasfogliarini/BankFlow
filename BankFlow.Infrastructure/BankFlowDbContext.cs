using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Wolverine;

namespace BankFlow.Infrastructure;

public sealed class BankFlowDbContext(IMessageBus bus, DbContextOptions options) : DbContext(options), ICommitScope
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(thisAssembly);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker
           .Entries<AggregateRoot>()
           .Select(x => x.Entity)
           .Where(x => x.DomainEvents.Any())
           .ToList();

        var domainEvents = aggregates
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await bus.PublishAsync(domainEvent);
        }

        return result;
    }

    public int Commit()
    {
        return base.SaveChanges();
    }
}
