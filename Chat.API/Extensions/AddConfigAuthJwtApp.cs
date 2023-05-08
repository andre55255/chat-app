using Chat.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Chat.API.Extensions
{
    public static class AddConfigAuthJwtApp
    {
        private static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }

        public static IServiceCollection AddAuthJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = ConfigJwt.AuthenticationScheme;
                opt.DefaultChallengeScheme = ConfigJwt.AuthenticationScheme;
            })
           .AddJwtBearer(ConfigJwt.AuthenticationScheme, opt =>
           {
               opt.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   LifetimeValidator = LifetimeValidator,
                   IssuerSigningKey =
                       new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConfigAppSettings.JwtSecret])),
                   ClockSkew = TimeSpan.FromMinutes(30),
                   ValidIssuer = configuration[ConfigAppSettings.JwtValidIssuer],
                   ValidAudience = configuration[ConfigAppSettings.JwtValidAudience]
               };

               opt.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       {
                           context.Response.Headers.Add("Token-Expired", "true");
                       }
                       return Task.CompletedTask;
                   }
               };
           });

            return services;
        }
    }
}
