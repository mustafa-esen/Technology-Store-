using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReviewService.Domain.Entities;

public class ProductReview
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("productId")]
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("imageUrls")]
    public List<string> ImageUrls { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
