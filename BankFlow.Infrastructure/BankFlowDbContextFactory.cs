using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankFlow.Infrastructure;

/// <summary>
/// This class is used by EF Core tools to create a DbContext instance at design time. It is not used at runtime.
/// 
/// dotnet ef migrations add InitSchema --project BankFlow.Infrastructure
/// </summary>
public class BankFlowDbContextFactory : IDesignTimeDbContextFactory<BankFlowDbContext>
{
    public BankFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<BankFlowDbContext>();

        optionsBuilder.UseNpgsql(
            "Server=localhost;Database=BankFlow;Trusted_Connection=True;TrustServerCertificate=True");

        return new BankFlowDbContext(null, optionsBuilder.Options);
    }
}
