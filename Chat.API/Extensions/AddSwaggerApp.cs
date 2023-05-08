using Chat.Helpers;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Chat.API.Extensions
{
    public static class AddSwaggerApp
    {
        public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services, IConfiguration configuration)
        {
            string version = configuration[ConfigAppSettings.VersionApi];

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc($"v{version}", new OpenApiInfo
                {
                    Title = "Chat.API",
                    Version = $"Versão {version}",
                    Description = $"API para Chat usando SignalR<br/> RELEASE {version} {DateTime.Now.ToString("dd/MM/yyyy")}"
                });

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization. Bearer token.<br/>
                    <br/>Entre com 'Bearer'[space] e depois digite o valor do token obtido no login.
                    <br/>Exemplo: \""'Bearer 12345abcdef\"""
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
