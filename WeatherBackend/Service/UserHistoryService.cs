using MongoDB.Bson;
using MongoDB.Driver;
using WeatherBackend.Data;
using WeatherBackend.Models;

namespace WeatherBackend.Service
{
    public class UserHistoryService
    {
        private readonly IMongoCollection<UserSearchHistory> _userSearchHistory;
        private const int MAX_HISTORY_ENTRIES = 50;

        public UserHistoryService(MongoDbContext context)
        {
            _userSearchHistory = context.UserSearchHistories;
        }

        public async Task<List<UserSearchHistory>> GetUserSearchHistory(string userId)
        {
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            
            return await _userSearchHistory
                .Find(h => h.UserId == userId)
                .SortByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task<UserSearchHistory> AddSearchHistory(UserSearchHistory history)
        {
            
            ValidateSearchHistory(history);

            
            if (history.Timestamp == default)
            {
                history.Timestamp = DateTime.UtcNow;
            }

            
            history.Id = ObjectId.GenerateNewId().ToString();

            
            var recentDuplicate = await CheckForRecentDuplicateSearch(history);
            if (recentDuplicate != null)
            {
                return recentDuplicate;
            }

            
            await CleanupOldSearchHistories(history.UserId);

            
            await _userSearchHistory.InsertOneAsync(history);

            return history;
        }

        private async Task<UserSearchHistory?> CheckForRecentDuplicateSearch(UserSearchHistory newHistory)
        {
            
            var recentDuplicate = await _userSearchHistory.Find(h =>
                h.UserId == newHistory.UserId &&
                h.CityName.ToLower() == newHistory.CityName.ToLower() &&
                h.Timestamp > DateTime.UtcNow.AddHours(-1)
            ).FirstOrDefaultAsync();

            return recentDuplicate;
        }

        private async Task CleanupOldSearchHistories(string userId)
        {
            try
            {
                
                long count = await _userSearchHistory.CountDocumentsAsync(h => h.UserId == userId);

                if (count >= MAX_HISTORY_ENTRIES)
                {
                    
                    var oldestEntriesToRemove = await _userSearchHistory
                        .Find(h => h.UserId == userId)
                        .SortBy(h => h.Timestamp)
                        .Limit((int)(count - MAX_HISTORY_ENTRIES + 1))
                        .Project(h => h.Id)
                        .ToListAsync();

                    if (oldestEntriesToRemove.Any())
                    {
                        await _userSearchHistory.DeleteManyAsync(
                            Builders<UserSearchHistory>.Filter.In(h => h.Id, oldestEntriesToRemove)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CleanupOldSearchHistories: {ex.Message}");
            }
        }

        public async Task DeleteSearchHistory(string historyId, string userId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(historyId))
            {
                throw new ArgumentException("History ID cannot be empty", nameof(historyId));
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            // Delete specific history entry
            var result = await _userSearchHistory.DeleteOneAsync(
                h => h.Id == historyId && h.UserId == userId
            );

            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException("History entry not found or does not belong to the user");
            }
        }

        public async Task ClearAllUserSearchHistory(string userId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            // Delete all search histories for the user
            await _userSearchHistory.DeleteManyAsync(h => h.UserId == userId);
        }

        private void ValidateSearchHistory(UserSearchHistory history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history), "Search history cannot be null");
            }

            if (string.IsNullOrWhiteSpace(history.UserId))
            {
                throw new ArgumentException("User ID is required", nameof(history.UserId));
            }

            if (string.IsNullOrWhiteSpace(history.CityName))
            {
                throw new ArgumentException("City name is required", nameof(history.CityName));
            }
        }
    }
}