namespace Chat.Communication.ViewObjects.APISettings
{
    public class AppSettingsVO
    {
        public string[]? CorsUrls { get; set; } = null;
        public AuthSettingsVO? Auth { get; set; } = null;
        public JwtSettingsVO? Jwt { get; set; } = null;
        public string? VersionAPI { get; set; } = null;
        public MongoDBSettingsVO? MongoDB { get; set; } = null;
        public EmailSettingsVO? Email { get; set; } = null;
    }

    public class AuthSettingsVO
    {
        public int? AttemptsLoginError { get; set; }
        public int? AttemptsLoginFailedDaysBlock { get; set; }
        public int? TimeMinExpiresAccessToken { get; set; }
        public int? TimeMinExpiresRefreshToken { get; set; }
    }

    public class JwtSettingsVO
    {
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
        public string? Secret { get; set; }
    }

    public class MongoDBSettingsVO
    {
        public string? Uri { get; set; }
        public string? Database { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsSSL { get; set; }
    }

    public class EmailSettingsVO
    {
        public string? Login { get; set; }
        public string? Pass { get; set; }
        public int? Port { get; set; }
        public string? Server { get; set; }
    }
}
