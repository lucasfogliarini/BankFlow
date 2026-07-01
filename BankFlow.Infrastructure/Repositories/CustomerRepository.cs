using Microsoft.EntityFrameworkCore;

namespace BankFlow.Infrastructure.Repositories;

public class CustomerRepository(BankFlowDbContext dbContext) : Repository(dbContext), ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Customer>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Customer>().FirstOrDefaultAsync(c => c.Cpf == cpf, cancellationToken);
    }

    public void Add(Customer customer)
    {
        dbContext.Add(customer);
    }

    public void Update(Customer customer)
    {
        dbContext.Update(customer);
    }
}
