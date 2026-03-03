using Shop.Application.Customers.Commands;
using Shop.Domain.Entities;

namespace Shop.Application.Customers.Interfaces;

public interface ICustomerService
{
    Task<Customer> CreateAsync(CreateCustomerCommand request, CancellationToken cancellationToken);
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
