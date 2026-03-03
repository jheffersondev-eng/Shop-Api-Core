namespace Shop.Domain.Entities;

public sealed class Review
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string ProductId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Customer? Customer { get; set; }
}
