namespace BankFlow.Infrastructure.Repositories
{
    public abstract class Repository(BankFlowDbContext dbContext) : IRepository
    {
        public ICommitScope CommitScope => dbContext;
    }
}
