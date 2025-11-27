using MassTransit;

namespace ProductService.Domain.Entities;

public class Product
{
    public Product()
    {
        Id = NewId.NextGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
}
