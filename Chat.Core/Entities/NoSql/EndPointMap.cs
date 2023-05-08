using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Core.Entities.NoSql
{
    public class EndPointMap : BaseEntity
    {
        [BsonElement("route")]
        public string Route { get; set; }
        [BsonElement("verb")]
        public string Verb { get; set; }
        [BsonElement("roles")]
        public List<string> Roles { get; set; }
    }
}
