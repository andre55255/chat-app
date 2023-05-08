using Chat.Communication.ViewObjects.APISettings;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Chat.Infrastructure.ServicesImpl;

namespace Chat.API.Extensions
{
    public static class AddCorsApp
    {
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            IAPISettingsService apiSettingsService = new APISettingsService(configuration, null);
            AppSettingsVO settings = apiSettingsService.GetInfoAppSettings();

            services.AddCors(opt =>
            {
                opt.AddPolicy(ConfigPolicy.Cors, policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(settings.CorsUrls!)
                        .AllowCredentials()
                        .SetIsOriginAllowed((hosts) => true);
                });
            });

            return services;
        }
    }
}
