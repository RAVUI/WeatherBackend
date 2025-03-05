using WeatherBackend.Models;
using MongoDB.Driver;
using WeatherBackend.Data;

namespace WeatherBackend.Service
{
    public class UserHistoryService
    {
        private readonly IMongoCollection<UserSearchHistory> _userSearchHistory;

        public UserHistoryService(MongoDbContext context)
        {
            _userSearchHistory = context.UserSearchHistories;
        }

        public async Task<List<UserSearchHistory>> GetUserSearchHistory(string userId) =>
            await _userSearchHistory.Find(h => h.UserId == userId).ToListAsync();

        public async Task AddSearchHistory(UserSearchHistory history) =>
            await _userSearchHistory.InsertOneAsync(history);
    }
}
