using MongoDB.Driver;
using ReviewService.Domain.Entities;

namespace ReviewService.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<ProductReview> Reviews =>
        _database.GetCollection<ProductReview>("reviews");
}
