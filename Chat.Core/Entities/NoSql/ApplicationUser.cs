using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Core.Entities.NoSql
{
    public class ApplicationUser : BaseEntity
    {
        [BsonElement("first_name")]
        public string? FirstName { get; set; } = null;
        [BsonElement("last_name")]
        public string? LastName { get; set; } = null;
        [BsonElement("email")]
        public string? Email { get; set; } = null;
        [BsonElement("username")]
        public string? Username { get; set; } = null;
        [BsonElement("hash_password")]
        public string? PasswordHash { get; set; } = null;
        [BsonElement("refresh_token")]
        public string? RefreshToken { get; set; } = null;
        [BsonElement("token_pass")]
        public string? TokenPass { get; set; } = null;
        [BsonElement("attempt_access_login")]
        public int AttemptAccessLogin { get; set; } = 0;
        [BsonElement("lockout_date")]
        public DateTime? LockoutDate { get; set; } = null;
        [BsonElement("lockout_date_end")]
        public DateTime? LockoutDateEnd { get; set; } = null;
        [BsonElement("roles")]
        public List<Role>? Roles { get; set; } = null;
    }
}
