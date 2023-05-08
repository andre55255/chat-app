using Chat.Infrastructure.Data.NoSql;

namespace Chat.API.Extensions
{
    public static class AddMongoDbContextApp
    {
        public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MongoDbContext, MongoDbContext>();
            return services;
        }
    }
}
