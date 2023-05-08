using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Account;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public UserController(IUserService userService, ILogService logService)
        {
            _userService = userService;
            _logService = logService;
        }

        /// <summary>
        /// Create - Método para criar usuário no sistema, passar dados pelo body
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] UserCreateVO model)
        {
            try
            {
                UserReturnVO user = await _userService.CreateAsync(model);
                return StatusCode(StatusCodes.Status201Created,
                    APIResponseVO.Ok($"Usuário criado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de criação de usuário: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de criação de usuário"));
            }
        }

        /// <summary>
        /// Edit - Método para editar usuário no sistema, passar id pela rota e dados de usuário pelo body
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] UserEditVO model)
        {
            try
            {
                UserReturnVO user = await _userService.EditAsync(id, model);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Usuário editado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de edição de usuário: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de edição de usuário"));
            }
        }

        /// <summary>
        /// Delete - Método para deletar usuário no sistema, passar id pela rota
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            try
            {
                UserReturnVO user = await _userService.DeleteAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Usuário deletado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de deleção de usuário {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de deleção de usuário"));
            }
        }

        /// <summary>
        /// GetById - Método para listar usuário pelo id, passar id pela rota
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            try
            {
                UserReturnVO user = await _userService.GetByIdAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Usuário listado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar usuário pelo id {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar usuário pelo id {id}"));
            }
        }

        /// <summary>
        /// GetAll - Método para listar usuários filtrados, passar filtro opcional pela query
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterUserVO filter)
        {
            try
            {
                ListAllEntityVO<UserReturnVO> user = await _userService.GetAllAsync(filter);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Usuários listados com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar usuários: {StaticMethods.SerializeObject(filter)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar usuários"));
            }
        }
    }
}
