using AutoMapper;
using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Account;
using Chat.Communication.ViewObjects.APISettings;
using Chat.Communication.ViewObjects.Email;
using Chat.Communication.ViewObjects.Role;
using Chat.Communication.ViewObjects.User;
using Chat.Core.RepositoriesInterface.NoSql;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.IdGenerators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Chat.Infrastructure.ServicesImpl
{
    public class AccountService : IAccountService
    {
        private readonly IApplicationUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        private readonly ISendMailService _sendMailService;
        private readonly IAPISettingsService _apiSettingsService;
        private AppSettingsVO _appSettings;

        public AccountService(IAPISettingsService apiSettingsService, IApplicationUserRepository userRepo, IMapper mapper, ILogService logService, ISendMailService sendMailService, IRoleRepository roleRepo, IUserService userService)
        {
            _apiSettingsService = apiSettingsService;
            _userRepo = userRepo;
            _mapper = mapper;
            _logService = logService;
            _sendMailService = sendMailService;

            _appSettings = apiSettingsService.GetInfoAppSettings();
            _roleRepo = roleRepo;
            _userService = userService;
        }

        public async Task<LoginResponseVO> LoginAsync(LoginDataVO model)
        {
            try
            {
                UserReturnVO user = await _userRepo.GetByUsernameAsync(model.Username);
                if (user == null)
                    throw new NotFoundException($"Usuário não encontrado com o nome de usuário: {model.Username}");

                await ValidConsistenceLoginDataAsync(model, user);
                await SignInUserAsync(user, model.Password);

                LoginResponseVO response = await CreateTokenJwtAsync(user);
                response.User = _mapper.Map<UserFindVO>(user);
                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar login: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar login");
            }
        }

        public async Task<RefreshTokenDataVO> RefreshTokenAsync(RefreshTokenDataVO model)
        {
            try
            {
                ClaimsPrincipal claims = await ValidAccessTokenAsync(model);
                UserReturnVO userSave = await ValidRefreshTokenAsync(claims, model.RefreshToken);

                LoginResponseVO token = await CreateTokenJwtAsync(userSave);
                RefreshTokenDataVO response = new RefreshTokenDataVO
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                };
                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar atualização de token: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar atualização de token");
            }
        }

        public async Task<Result> ResetPasswordSignInUserAsync(ResetPasswordSignInVO model)
        {
            try
            {
                UserReturnVO userSave = await VerifyOldPasswordCorrectAsync(model);
                Result result = await SetNewPasswordAsync(userSave, model.NewPassword);
                return result;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar alteração de senha de usuário logado: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar alteração de senha de usuário logado");
            }
        }

        public async Task<UserInfoVO> GetUserInfoAsync()
        {
            try
            {
                CurrentRequestVO currentRequest = _apiSettingsService.GetInfoCurrentRequest(true);
                UserReturnVO userSave = await _userRepo.GetByIdAsync(currentRequest.CurrentUser.Id);
                if (userSave == null)
                    throw new NotFoundException($"Usuário não encontrado com {currentRequest.CurrentUser.Username}");

                UserInfoVO response = _mapper.Map<UserInfoVO>(userSave);
                return response;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao listar dados de usuário logado",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao listar dados de usuário logado");
            }
        }

        public async Task<Result> ForgotPasswordSendMailAsync(ForgotPasswordVO model)
        {
            try
            {
                UserReturnVO userSave = await _userRepo.GetByUsernameAsync(model.UserName);
                if (userSave == null)
                    throw new NotFoundException($"Não foi encontrado um usuário com {model.UserName}");

                string newPassword = StaticMethods.GenerateAlfaNumericRandom(10);
                EmailDataForgotPasswordVO emailData = new EmailDataForgotPasswordVO
                {
                    Recipients = new List<string> { userSave.Email },
                    NewPassword = newPassword
                };
                Result resultSendMail = await _sendMailService.SendMailForgotPasswordAsync(emailData, userSave);
                if (resultSendMail.IsFailed)
                    return resultSendMail;

                Result resultUpdatedPass = await SetNewPasswordAsync(userSave, newPassword);
                if (resultUpdatedPass.IsFailed)
                    return resultUpdatedPass;

                return Result.Ok().WithSuccess($"Foi enviado um email para {userSave.Email} com a nova senha para login");
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina esqueci minha senha: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao executar rotina de esqueci minha senha");
            }
        }

        public async Task<Result> RegisterUserPublicAsync(RegisterUserPublicVO model)
        {
            try
            {
                UserCreateVO userCreate = _mapper.Map<UserCreateVO>(model);
                RoleReturnVO role = await _roleRepo.GetByNameAsync(Roles.User);
                if (role == null)
                    throw new NotFoundException($"Perfil não encontrado para vincular ao usuário");

                userCreate.Roles = new List<string> { role.Id };
                await _userService.CreateAsync(userCreate);
                
                return Result.Ok();
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao inserir novo usuário, {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao inserir novo usuário");
            }
        }

        private async Task ValidConsistenceLoginDataAsync(LoginDataVO model, UserReturnVO user)
        {
            try
            {
                if (_appSettings == null ||
                    _appSettings.Auth == null)
                    throw new ValidException($"Falha inesperada ao pegar dados de configuração de autenticação");

                int attemptsLoginFailed = _appSettings.Auth.AttemptsLoginError.Value;

                if (user.AttemptAccessLogin >= attemptsLoginFailed)
                    throw new ValidException($"Sua senha está bloqueada por excesso de tentivas erradas, recupere-a e tente novamente. Usuário: {user.Username}");

                if (user.LockoutDateEnd.HasValue && user.LockoutDateEnd > DateTime.Now)
                    throw new ValidException($"Conta está bloqueada, recupere sua senha ou aguarda até a data de desbloqueio: {user.LockoutDateEnd.Value.ToString("dd/MM/yyyy HH:mm")}. Usuário: {user.Username}");
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar consistência de dados de login: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar consistência de dados de login");
            }
        }

        private async Task SignInUserAsync(UserReturnVO user, string password)
        {
            try
            {
                string salt = StaticMethods.GetSaltGenerateHashPasswordUser(user, password);
                string hashPassword = StaticMethods.CryptPasswordMD5(password, salt);

                if (user.PasswordHash == hashPassword)
                    await LoginSuccessTreatmentAsync(user);
                else
                {
                    await LoginFailedTreatmentAsync(user);
                    throw new ValidException($"Senha não confere");
                }
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar autenticação: ${StaticMethods.SerializeObject(user)} - {password}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar autenticação");
            }
        }

        private async Task LoginSuccessTreatmentAsync(UserReturnVO user)
        {
            try
            {
                await _userRepo.SetBlockUnblockUserAsync(user.Id, true);
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar tratamento na base de dados para login com sucesso: ${StaticMethods.SerializeObject(user)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar tratamento na base de dados para login com sucesso");
            }
        }

        private async Task LoginFailedTreatmentAsync(UserReturnVO user)
        {
            try
            {
                await _userRepo.SetAttemptLoginAsync(user.Id, true);

                int newAttemptLoginFailed = user.AttemptAccessLogin + 1;
                if (newAttemptLoginFailed >= _appSettings.Auth.AttemptsLoginError)
                {
                    DateTime dateLockoutEnd = DateTime.Now.AddDays(_appSettings.Auth.AttemptsLoginFailedDaysBlock.Value);
                    await _userRepo.SetBlockUnblockUserAsync(user.Id, false, dateLockoutEnd);
                }
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar tratamento na base de dados para login com falha: ${StaticMethods.SerializeObject(user)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar tratamento na base de dados para login com falha");
            }
        }

        private async Task<LoginResponseVO> CreateTokenJwtAsync(UserReturnVO user)
        {
            try
            {
                List<Claim> claims = await GetClaimsListTokenAsync(user);

                string secretKey = _appSettings.Jwt.Secret;
                SymmetricSecurityKey securityKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                SigningCredentials credentials =
                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                DateTime expiresIn = DateTime.Now.AddMinutes(_appSettings.Auth.TimeMinExpiresAccessToken.Value);
                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _appSettings.Jwt.ValidIssuer,
                    audience: _appSettings.Jwt.ValidAudience,
                    expires: expiresIn,
                    claims: claims,
                    signingCredentials: credentials
                );
                string refreshToken = await GenerateRefreshTokenAndInsertDbAsync(user);

                LoginResponseVO response = new LoginResponseVO();
                JwtSecurityTokenHandler handlerToken = new JwtSecurityTokenHandler();
                response.AccessToken = handlerToken.WriteToken(token);
                response.RefreshToken = refreshToken;
                response.ExpirationAt = token.ValidTo.AddHours(-3);

                return response;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar geração de token de acesso: ${StaticMethods.SerializeObject(user)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar geração de token de acesso");
            }
        }

        private async Task<string> GenerateRefreshTokenAndInsertDbAsync(UserReturnVO user)
        {
            try
            {
                string salt = $"{user.Email}@{DateTime.Now.AddMilliseconds(-100).Ticks}";
                string token = StaticMethods.CryptPasswordMD5(user.PasswordHash, salt);

                await _userRepo.SetRefreshTokenAsync(user.Id, token);
                return token;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw new ValidException(ex.Message, ex);
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar geração de refrsh token de acesso: ${StaticMethods.SerializeObject(user)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar geração de refresh token de acesso");
            }
        }

        private async Task<List<Claim>> GetClaimsListTokenAsync(UserReturnVO user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                };
                foreach (RoleReturnVO role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name.ToUpper()));
                }
                return claims;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao montar payload de token de acesso: ${StaticMethods.SerializeObject(user)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao montar payload de token de acesso");
            }
        }

        private async Task<ClaimsPrincipal> ValidAccessTokenAsync(RefreshTokenDataVO model)
        {
            try
            {
                if (_appSettings == null ||
                    _appSettings.Auth == null)
                    throw new ValidException($"Não foram encontradas as configuraões de autenticação");

                string secretKey = _appSettings.Jwt.Secret;
                SymmetricSecurityKey securityKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                TokenValidationParameters validationToken = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                       new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = false
                };
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                ClaimsPrincipal claims =
                    tokenHandler.ValidateToken(model.AccessToken, validationToken, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
                    throw new ValidException($"Token informado inválido");

                return claims;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar validação de token de acesso: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar validação de token de acesso");
            }
        }

        private async Task<UserReturnVO> ValidRefreshTokenAsync(ClaimsPrincipal claims, string refreshToken)
        {
            try
            {
                string? username =
                    claims.Claims
                          .Where(x => x.Type == ClaimTypes.Name)
                          .Select(x => x.Value)
                          .FirstOrDefault();

                if (username == null)
                    throw new ValidException($"Usuário não encontrado no payload do token");

                UserReturnVO userSave = await _userRepo.GetByUsernameAsync(username);
                if (userSave == null)
                    throw new NotFoundException($"Usuário {username} não encontrado na base de dados");

                if (userSave.RefreshToken != refreshToken)
                    throw new ValidException($"Refresh token informado não corresponde ao cadastrado na base de dados, verifique");

                return userSave;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar validação de token de refesh: ${StaticMethods.SerializeObject(claims)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar validação de token de refesh");
            }
        }

        private async Task<UserReturnVO> VerifyOldPasswordCorrectAsync(ResetPasswordSignInVO model)
        {
            try
            {
                CurrentRequestVO currentRequest = _apiSettingsService.GetInfoCurrentRequest(true);
                UserReturnVO userSave = await _userRepo.GetByIdAsync(currentRequest.CurrentUser.Id);
                if (userSave == null)
                    throw new NotFoundException($"Usuário não encontrado");

                string saltPassword = StaticMethods.GetSaltGenerateHashPasswordUser(userSave, model.OldPassword);
                string oldPasswordHash = StaticMethods.CryptPasswordMD5(model.OldPassword, saltPassword);
                if (oldPasswordHash != userSave.PasswordHash)
                    throw new ValidException($"Senha antiga incorreta");

                return userSave;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar verificação de senha antiga: ${StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar verificação de senha antiga");
            }
        }

        private async Task<Result> SetNewPasswordAsync(UserReturnVO user, string newPassword)
        {
            try
            {
                string salt = StaticMethods.GetSaltGenerateHashPasswordUser(user, newPassword);
                string newPasswordHash = StaticMethods.CryptPasswordMD5(newPassword, salt);

                Result result = await _userRepo.SetNewPasswordAsync(user.Id, newPasswordHash);
                return result;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao realizar troca de senha: {StaticMethods.SerializeObject(user)} - {newPassword}",
                    this.GetType().ToString());

                throw new ValidException("Falha inesperada ao realizar troca de senha na base de dados");
            }
        }
    }
}
