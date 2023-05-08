namespace Chat.API.Extensions
{
    public static class AddSignalRApp
    {
        public static IServiceCollection AddSignalRServiceApp(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }
    }
}
