using MongoDB.Driver;
using WeatherBackend.Data;
using WeatherBackend.Models;
using WeatherBackend.Data;

namespace WeatherBackend.Service
{
    public class FavoriteCityService
    {
        private readonly IMongoCollection<FavoriteCity> _favoriteCities;

        public FavoriteCityService(MongoDbContext context)
        {
            _favoriteCities = context.FavoriteCities;
        }

        public async Task<List<FavoriteCity>> GetFavoriteCities(string userId) =>
            await _favoriteCities.Find(c => c.UserId == userId).ToListAsync();

        public async Task AddFavoriteCity(FavoriteCity city) =>
            await _favoriteCities.InsertOneAsync(city);

        public async Task RemoveFavoriteCity(string cityId) =>
            await _favoriteCities.DeleteOneAsync(c => c.Id == cityId);
    }
}
