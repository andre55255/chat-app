using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Infrastructure.RepositoriesImpl.NoSql;

namespace Chat.API.Extensions
{
    public static class AddRepositoriesApp
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IEndPointRepository, EndPointRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
