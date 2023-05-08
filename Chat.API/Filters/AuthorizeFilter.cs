using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Communication.ViewObjects.APISettings;
using Chat.Communication.ViewObjects.EndPoints;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chat.API.Filters
{
    public class AuthorizeFilter : IActionFilter
    {
        private readonly IEndPointRepository _endpointRepo;
        private readonly IAPISettingsService _apiSettingsService;
        private readonly ILogService _logService;
        private AppSettingsVO _appSettings;

        public AuthorizeFilter(IEndPointRepository endpointRepo, IAPISettingsService apiSettingsService, ILogService logService)
        {
            _endpointRepo = endpointRepo;
            _apiSettingsService = apiSettingsService;
            _logService = logService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Method executed after controller
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Method executed before controller
            try
            {
                _appSettings = _apiSettingsService.GetInfoAppSettings();

                AuthorizeNoSql(context);
            }
            catch (RepositoryException ex)
            {
                ObjectResult result = new ObjectResult(APIResponseVO.Fail(ex.Message));
                result.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = result;
                return;
            }
            catch (NotFoundException ex)
            {
                ObjectResult result = new ObjectResult(APIResponseVO.Fail(ex.Message));
                result.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = result;
                return;
            }
            catch (ValidException ex)
            {
                ObjectResult result = new ObjectResult(APIResponseVO.Fail(ex.Message));
                result.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = result;
                return;
            }
            catch (Exception ex)
            {
                _logService.WriteSync(ex, ex.Message, this.GetType().ToString());
                ObjectResult result = null;
                if (ex.Message.Contains("Lifetime"))
                {
                    result = new ObjectResult(APIResponseVO.Fail("Token expirado"));
                }
                else
                {
                    result = new ObjectResult(APIResponseVO.Fail("Falha no filtro de autorização"));
                }
                result.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = result;
                return;
            }
        }

        private void AuthorizeNoSql(ActionExecutingContext context)
        {
            try
            {
                string route = "";
                string[] routeSplited = context.HttpContext.Request.Path.Value.Split("/");
                if (routeSplited.Length > 2)
                {
                    route += $"{routeSplited[0]}/{routeSplited[1]}";
                    
                    try
                    {
                        StaticMethods.IsObjectIdValid(routeSplited[2]);
                    }
                    catch (Exception ex)
                    {
                        route += $"/{routeSplited[2]}";
                    }
                }
                else
                    route = context.HttpContext.Request.Path.Value;

                string verb = context.HttpContext.Request.Method;

                EndPointSaveVO response = 
                    _endpointRepo.GetByRouteByVerbSync(route, verb);

                if (response == null)
                    return;

                VerifyTokenJwt(context);

                ValidAuthorize(context, response.Roles);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ValidException("Falha no filtro de autorização sql server", ex);
            }
        }

        private void ValidAuthorize(ActionExecutingContext context, List<string> rolesAccess)
        {
            try
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    bool flagAllowAccess = false;

                    IEnumerable<Claim> claimRoles = context.HttpContext.User.FindAll(ClaimTypes.Role);

                    foreach (Claim item in claimRoles)
                    {
                        if (rolesAccess.Contains(item.Value.ToUpper()))
                            flagAllowAccess = true;
                    }

                    if (!flagAllowAccess)
                    {
                        ObjectResult result = new ObjectResult(APIResponseVO.Fail("Não autorizado"));
                        result.StatusCode = StatusCodes.Status403Forbidden;
                        context.Result = result;
                        return;
                    }
                }
                else
                {
                    ObjectResult result = new ObjectResult(APIResponseVO.Fail("Não autenticado"));
                    result.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = result;
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new ValidException("Validação se está autorizado falhou", ex);
            }
        }

        private void VerifyTokenJwt(ActionExecutingContext context)
        {
            try
            {
                
                string bearerToken = context.HttpContext
                                          .Request
                                          .Headers
                                          .Where(x => x.Key == "Authorization" ||
                                                      x.Key == "authorization")
                                          .Select(x => x.Value)
                                          .FirstOrDefault();

                if (bearerToken == null)
                    throw new ValidException("Token não informado na requisição");

                string[] bearerTokenSplited = bearerToken.Split(" ");
                if (bearerTokenSplited[0].ToUpper() != "BEARER")
                    throw new ValidException("Token não informado corretamente. Ex: Bearer [[token]]");

                string token = bearerTokenSplited[1];

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                context.HttpContext.User = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = LifetimeValidator,
                    IssuerSigningKey =
                       new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(30),
                    ValidIssuer = _appSettings.Jwt.ValidIssuer,
                    ValidAudience = _appSettings.Jwt.ValidAudience
                }, out SecurityToken validatedToken);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime"))
                    throw new ValidException("Token expirado");
                else
                    throw new ValidException("Token jwt inválido", ex);
            }
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }

            return false;
        }
    }
}
