using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

public sealed class Address
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public AddressType Type { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Customer? Customer { get; set; }
}
