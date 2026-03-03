using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<OrderReview> Reviews { get; set; } = new List<OrderReview>();
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
