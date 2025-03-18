using MongoDB.Driver;
using WeatherBackend.Data;
using WeatherBackend.Models;

namespace WeatherBackend.Service
{
    public class FavoriteCityService
    {
        private readonly IMongoCollection<FavoriteCity> _favoriteCities;

        public FavoriteCityService(MongoDbContext context)
        {
            _favoriteCities = context.FavoriteCities;
        }

        public async Task<List<FavoriteCity>> GetFavoriteCities(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            return await _favoriteCities.Find(c => c.UserId == userId).ToListAsync();
        }

        public async Task<FavoriteCity> AddFavoriteCity(FavoriteCity city)
        {
            
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city), "Favorite city cannot be null");
            }

            if (string.IsNullOrWhiteSpace(city.UserId))
            {
                throw new ArgumentException("User ID is required", nameof(city.UserId));
            }

            if (string.IsNullOrWhiteSpace(city.CityName))
            {
                throw new ArgumentException("City name is required", nameof(city.CityName));
            }

            
            var existingCity = await _favoriteCities.Find(c =>
                c.UserId == city.UserId &&
                c.CityName.ToLower() == city.CityName.ToLower())
                .FirstOrDefaultAsync();

            if (existingCity != null)
            {
                throw new InvalidOperationException("City already exists in favorites");
            }
            city.Id = null;

           
            if (city.AddedDate == default)
            {
                city.AddedDate = DateTime.UtcNow;
            }

            
            await _favoriteCities.InsertOneAsync(city);

            return city;
        }

        public async Task RemoveFavoriteCity(string cityId, string userId)
        {
            if (string.IsNullOrWhiteSpace(cityId))
            {
                throw new ArgumentException("City ID cannot be empty", nameof(cityId));
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            
            var result = await _favoriteCities.DeleteOneAsync(c => c.Id == cityId && c.UserId == userId);

            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("City not found or does not belong to the user");
            }
        }
    }
}