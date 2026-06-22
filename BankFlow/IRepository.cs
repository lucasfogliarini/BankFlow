namespace BankFlow;

public interface IRepository
{
    ICommitScope CommitScope { get; }
}

public interface ICommitScope
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    int Commit();
}
