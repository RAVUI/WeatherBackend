using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherBackend.Models
{
    public class UserSearchHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("CityName")]
        public string CityName { get; set; } = string.Empty;

        [BsonElement("Temperature")]
        public double Temperature { get; set; }

        [BsonElement("Humidity")]
        public double Humidity { get; set; }

        [BsonElement("WindSpeed")]
        public double WindSpeed { get; set; }

        [BsonElement("WeatherCondition")]
        public string WeatherCondition { get; set; } = string.Empty;

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}