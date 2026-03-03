using Shop.Domain.Entities;

namespace Shop.Application.Customers.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Customer> GetByCognitoSubAsync(string cognitoSub, CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
