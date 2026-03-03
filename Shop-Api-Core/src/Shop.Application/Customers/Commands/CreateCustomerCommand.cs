using System.ComponentModel.DataAnnotations;
using MediatR;
using Shop.Application.Customers.Models;

namespace Shop.Application.Customers.Commands;

public sealed record CreateCustomerCommand : IRequest<CustomerResponse>
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    public string Document { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Phone]
    [StringLength(30)]
    public string Phone { get; set; }
}
