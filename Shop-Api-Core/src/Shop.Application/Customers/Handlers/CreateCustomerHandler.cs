using MediatR;
using Shop.Application.Customers.Commands;
using Shop.Application.Customers.Interfaces;
using Shop.Application.Customers.Models;
using Shop.Domain.Entities;

namespace Shop.Application.Customers.Handlers;

public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<CustomerResponse> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {  
        Customer customer = await _customerService.CreateAsync(request, cancellationToken);
        return CustomerResponse.From(customer);
    }
}
