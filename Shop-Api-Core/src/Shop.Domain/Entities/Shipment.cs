using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

public sealed class Shipment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Carrier { get; set; }
    public string TrackingCode { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Order? Order { get; set; }
}
