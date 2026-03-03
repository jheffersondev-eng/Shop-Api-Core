using Shop.Domain.Entities;

namespace Shop.Application.Customers.Models;

public sealed record CustomerResponse(
    Guid Id,
    string CognitoSub,
    string Email,
    string Name,
    string Phone,
    DateTimeOffset CreatedAt)
{
    public static CustomerResponse From(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.CognitoSub,
            customer.Email,
            customer.Name,
            customer.Phone,
            customer.CreatedAt);
    }
}
