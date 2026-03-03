namespace Shop.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; set; }
    public string CognitoSub { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
