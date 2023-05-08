using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Chat.Core.Entities.NoSql
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("message")]
        public string? Message { get; set; } = null;
        [BsonElement("exception")]
        public string? Exception { get; set; } = null;
        [BsonElement("place")]
        public string? Place { get; set; } = null;
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
