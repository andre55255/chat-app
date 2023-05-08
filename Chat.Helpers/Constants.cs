namespace Chat.Helpers
{
    public static class Roles
    {
        public static string Admin = "ADMIN";
        public static string User = "USER";
    }

    public static class ConfigPolicy
    {
        public static string Cors = "ClientPermission";
    }

    public static class DirectoriesName
    {
        public static string EmailTemplates = "Content/Templates";
    }

    public static class ConstantsEmail
    {
        public static string TemplateResetAccount = "ResetPassword.html";
        public static string SubjectResetAccount = $"Chat - Email de redefinição de senha - ";
    }

    public static class ConfigJwt
    {
        public static string AuthenticationScheme = "JwtBearer";
    }

    public static class ConfigAppSettings
    {
        public static string AuthSection = "Auth";
        public static string AuthAttemptsLoginError = "Auth:AttemptsLoginError";
        public static string AuthAttemptsLoginFailedDaysBlock = "Auth:AttemptsLoginFailedDaysBlock";
        public static string AuthTimeMinExpiresAccessToken = "Auth:TimeMinExpiresAccessToken";
        public static string AuthTimeMinExpiresRefreshToken = "Auth:TimeMinExpiresRefreshToken";

        public static string JwtSection = "Jwt";
        public static string JwtSecret = "Jwt:Secret";
        public static string JwtValidIssuer = "Jwt:ValidIssuer";
        public static string JwtValidAudience = "Jwt:ValidAudience";

        public static string CorsUrls = "CorsUrls";
        public static string VersionApi = "VersionAPI";

        public static string MongoDBSection = "MongoDB";
        public static string MongoDBUri = "MongoDB:Uri";
        public static string MongoDBDatabase = "MongoDB:Database";
        public static string MongoDBUsername = "MongoDB:Username";
        public static string MongoDBPassword = "MongoDB:Password";
        public static string MongoDBIsSSL = "MongoDB:IsSSL";

        public static string Email = "Email";
        public static string EmailLogin = "Email:Login";
        public static string EmailPass = "Email:Pass";
        public static string EmailPort = "Email:Port";
        public static string EmailServer = "Email:Server";
    }
}
