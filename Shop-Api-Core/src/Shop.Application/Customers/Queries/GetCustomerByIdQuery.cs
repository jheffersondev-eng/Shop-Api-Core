using MediatR;
using Shop.Application.Customers.Models;

namespace Shop.Application.Customers.Queries;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerResponse>;
