using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Chat.Core.Entities.NoSql
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [BsonElement("disabled_at")]
        public DateTime? DisabledAt { get; set; } = null;
    }
}
