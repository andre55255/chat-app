using Chat.Core.ServicesInterface;
using Chat.Infrastructure.ServicesImpl;

namespace Chat.API.Extensions
{
    public static class AddServicesApp
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAPISettingsService, APISettingsService>();
            services.AddScoped<IEndPointService, EndPointService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ISendMailService, SendMailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
