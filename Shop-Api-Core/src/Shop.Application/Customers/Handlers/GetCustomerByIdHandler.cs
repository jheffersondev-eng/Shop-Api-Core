using MediatR;
using Shop.Application.Customers.Interfaces;
using Shop.Application.Customers.Models;
using Shop.Application.Customers.Queries;
using Shop.Domain.Entities;

namespace Shop.Application.Customers.Handlers;

public sealed class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
{
    private readonly ICustomerService _customerService;

    public GetCustomerByIdHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<CustomerResponse> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        Customer customer = await _customerService.GetByIdAsync(request.Id, cancellationToken);
        return customer is null ? null : CustomerResponse.From(customer);
    }
}
