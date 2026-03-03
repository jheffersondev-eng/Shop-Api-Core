namespace Shop.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ProductId { get; set; }
    public string ProductNameSnapshot { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Order? Order { get; set; }
}
