using Microsoft.EntityFrameworkCore;
using Shop.Application.Customers.Interfaces;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ShopDbContext _dbContext;

    public CustomerRepository(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.Id == id && customer.DeletedAt == null,
                cancellationToken);
    }

    public Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.Email == email && customer.DeletedAt == null,
                cancellationToken);
    }

    public Task<Customer> GetByCognitoSubAsync(string cognitoSub, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.CognitoSub == cognitoSub && customer.DeletedAt == null,
                cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
