using MongoDB.Bson;
using MongoDB.Driver;

namespace Vladify.IntegrationTests.Infrastructure;

public class TestDataSeeder(IMongoDatabase _database)
{
    public async Task ResetDataAsync()
    {
        var collections = await _database.ListCollectionNamesAsync();
        var collectionNames = await collections.ToListAsync();

        foreach (var collectionName in collectionNames)
        {
            await _database
                .GetCollection<BsonDocument>(collectionName)
                .DeleteManyAsync(Builders<BsonDocument>.Filter.Empty);
        }
    }

    public async Task<TEntity> SeedDataAsync<TEntity>(string collectionName, TEntity entity) where TEntity : class
    {
        var collection = _database.GetCollection<TEntity>(collectionName);
        await collection.InsertOneAsync(entity);

        return entity;
    }
}
