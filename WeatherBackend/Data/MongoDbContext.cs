using WeatherBackend.Configurations;
using WeatherBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace WeatherBackend.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        public IMongoCollection<UserSearchHistory> UserSearchHistories =>
            _database.GetCollection<UserSearchHistory>("UserSearchHistory");

        public IMongoCollection<FavoriteCity> FavoriteCities =>
            _database.GetCollection<FavoriteCity>("FavoriteCities");
    }
}
