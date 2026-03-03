namespace Shop.Domain.Entities;

public sealed class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public string ProductId { get; set; }
    public string ProductNameSnapshot { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Cart? Cart { get; set; }
}
