using Shop.Application.Customers.Commands;

namespace Shop.Application.Customers.Interfaces;

public interface ICognitoUserService
{
    Task<string> CreateUserAsync(CreateCustomerCommand request, CancellationToken cancellationToken);
}
