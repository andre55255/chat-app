using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Account;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accService;
        private readonly ILogService _logService;

        public AccountController(IAccountService accService, ILogService logService)
        {
            _accService = accService;
            _logService = logService;
        }

        /// <summary>
        /// Login - Método para realizar login de usuário no sistema, passar dados pelo body
        /// </summary>        
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDataVO model)
        {
            try
            {
                LoginResponseVO result = await _accService.LoginAsync(model);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Login efetuado com sucesso", result));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de login: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de login"));
            }
        }

        /// <summary>
        /// Refresh - Método para realizar atualização de token de usuário no sistema, passar dados pelo body
        /// </summary>
        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenDataVO model)
        {
            try
            {
                RefreshTokenDataVO result = await _accService.RefreshTokenAsync(model);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Token atualizado com sucesso", result));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de refresh token: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de refresh token"));
            }
        }

        /// <summary>
        /// ResetPasswordSignIn - Método para realizar recuperação de senha de usuário logado, passar dados pelo body
        /// </summary>
        [HttpPost]
        [Route("ResetPasswordSignIn")]
        public async Task<IActionResult> ResetPasswordSignInAsync([FromBody] ResetPasswordSignInVO model)
        {
            try
            {
                Result result = await _accService.ResetPasswordSignInUserAsync(model);
                if (result.IsFailed)
                    throw new ValidException(StaticMethods.ExtractResultMessage(result));

                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Senha atualizada com sucesso"));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de recuperação de senha logado: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de recuperação de senha logado"));
            }
        }

        /// <summary>
        /// UserAuthInfo - Método para realizar recuperação de dados de usuário logado, sem parâmetros
        /// </summary>
        [HttpGet]
        [Route("UserAuthInfo")]
        public async Task<IActionResult> UserAuthInfoAsync()
        {
            try
            {
                UserInfoVO result = await _accService.GetUserInfoAsync();
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Dados de usuário logado listados com sucesso", result));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de listagem de dados de usuário logado",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listagem de dados de usuário logado"));
            }
        }

        /// <summary>
        /// ForgotPassword - Método para realizar reset de senha, passar dados pelo body
        /// </summary>
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordVO model)
        {
            try
            {
                Result result = await _accService.ForgotPasswordSendMailAsync(model);
                if (result.IsFailed)
                    throw new ValidException(StaticMethods.ExtractResultMessage(result));

                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok(StaticMethods.ExtractResultMessage(result)));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de esqueci minha senha: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de esqueci minha senha"));
            }
        }

        /// <summary>
        /// SignUpUser - Método para criar um novo usuário público no sistema, passar dados no body
        /// </summary>
        [HttpPost]
        [Route("SignUpUser")]
        public async Task<IActionResult> SignUpUserAsync([FromBody] RegisterUserPublicVO model)
        {
            try
            {
                Result result = await _accService.RegisterUserPublicAsync(model);
                if (result.IsFailed)
                    throw new ValidException(StaticMethods.ExtractResultMessage(result));

                return StatusCode(StatusCodes.Status201Created,
                    APIResponseVO.Ok($"Usuário {model.Username} criado com sucesso"));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (ValidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    APIResponseVO.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de registrar usuário público: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de registrar usuário"));
            }
        }
    }
}
