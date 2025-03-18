using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherBackend.Models
{
    public class FavoriteCity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("CityName")]
        public string CityName { get; set; } = string.Empty;

        [BsonElement("AddedDate")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
