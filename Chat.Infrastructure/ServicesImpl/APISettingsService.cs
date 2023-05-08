using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.APISettings;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Chat.Infrastructure.ServicesImpl
{
    public class APISettingsService : IAPISettingsService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor? _httpContext;

        public APISettingsService(IConfiguration configuration, IHttpContextAccessor? httpContext = null)
        {
            _configuration = configuration;
            _httpContext = httpContext;
        }

        public AppSettingsVO GetInfoAppSettings()
        {
            try
            {
                if (_configuration == null)
                    throw new ValidException($"Objeto de configurações não encontrado");

                AppSettingsVO settings = new AppSettingsVO();
                settings.CorsUrls = _configuration.GetSection(ConfigAppSettings.CorsUrls)
                                                  .Get<string[]>();

                settings.Auth = _configuration.GetSection(ConfigAppSettings.AuthSection)
                                              .Get<AuthSettingsVO>();

                settings.Jwt = _configuration.GetSection(ConfigAppSettings.JwtSection)
                                             .Get<JwtSettingsVO>();

                settings.VersionAPI = _configuration[ConfigAppSettings.VersionApi];
                settings.MongoDB = _configuration.GetSection(ConfigAppSettings.MongoDBSection)
                                                 .Get<MongoDBSettingsVO>();

                settings.Email = _configuration.GetSection(ConfigAppSettings.Email)
                                               .Get<EmailSettingsVO>();

                return settings;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ValidException($"Falha inesperada ao pegar informações de configuração", ex);
            }
        }

        public CurrentRequestVO GetInfoCurrentRequest(bool isAuthenticatedRequest = false)
        {
            try
            {
                if (_httpContext == null || _httpContext.HttpContext == null)
                    throw new ValidException($"Objeto de contexto de requisição não encontrado");

                CurrentRequestVO response = new CurrentRequestVO();
                response.BaseUrl =
                    _httpContext.HttpContext
                                .Request
                                .Headers
                                .Where(x => x.Key.ToUpper() == "ORIGIN")
                                .Select(x => x.Value)
                                .FirstOrDefault();

                response.CurrentUser = isAuthenticatedRequest ? GetCurrentUser(_httpContext.HttpContext.Request.HttpContext.User) : null;
                return response;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ValidException($"Falha inesperada ao pegar informações da requisição atual", ex);
            }
        }

        private CurrentUserVO GetCurrentUser(ClaimsPrincipal user)
        {
            try
            {
                CurrentUserVO response = new CurrentUserVO();
                if (user.Claims == null)
                    throw new ValidException($"Não foi possível obter o payload do token com as claims");

                response.Id =
                    user.Claims
                        .Where(x => x.Type == ClaimTypes.NameIdentifier)
                        .Select(x => x.Value)
                        .FirstOrDefault();

                if (response.Id == null)
                    throw new ValidException($"Não foi encontrado o id do usuário no payload");

                response.Username =
                    user.Claims
                        .Where(x => x.Type == ClaimTypes.Name)
                        .Select(x => x.Value)
                        .FirstOrDefault();

                if (response.Username == null)
                    throw new ValidException($"Não foi encontrado o nome do usuário no payload");

                response.Email =
                   user.Claims
                       .Where(x => x.Type == ClaimTypes.Email)
                       .Select(x => x.Value)
                       .FirstOrDefault();

                if (response.Email == null)
                    throw new ValidException($"Não foi encontrado o email no payload");

                response.Roles =
                    user.Claims
                        .Where(x => x.Type == ClaimTypes.Role)
                        .Select(x => x.Value)
                        .ToList();

                if (response.Roles == null || response.Roles.Count == 0)
                    throw new ValidException($"Não foram encontrados perfis do usuário no payload");

                return response;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ValidException($"Falha inesperada ao pegar dados de usuário logado", ex);
            }
        }
    }
}
