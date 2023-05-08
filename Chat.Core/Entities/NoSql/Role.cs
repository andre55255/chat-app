using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Chat.Core.Entities.NoSql
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null;
        [BsonElement("name")]
        public string? Name { get; set; } = null;
        [BsonElement("normalized_name")]
        public string? NormalizedName { get; set; } = null;
    }
}
