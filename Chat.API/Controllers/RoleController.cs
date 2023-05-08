using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Communication.ViewObjects.Role;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogService _logService;

        public RoleController(IRoleService roleService, ILogService logService)
        {
            _roleService = roleService;
            _logService = logService;
        }

        /// <summary>
        /// Create - Método para criar perfil no sistema, passar dados pelo body
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RoleSaveVO model)
        {
            try
            {
                RoleReturnVO user = await _roleService.CreateAsync(model);
                return StatusCode(StatusCodes.Status201Created,
                    APIResponseVO.Ok($"Perfil criado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de criação de perfil: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de criação de perfil"));
            }
        }

        /// <summary>
        /// Edit - Método para editar perfil no sistema, passar id pela rota e dados de perfil pelo body
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] RoleSaveVO model)
        {
            try
            {
                RoleReturnVO user = await _roleService.EditAsync(id, model);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Perfil editado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de edição de perfil: {id} - {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de edição de perfil"));
            }
        }

        /// <summary>
        /// Delete - Método para deletar perfil no sistema, passar id pela rota
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            try
            {
                RoleReturnVO user = await _roleService.DeleteAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Perfil deletado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de deleção de perfil {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de deleção de perfil"));
            }
        }

        /// <summary>
        /// GetById - Método para lisatr perfil pelo id, passar id pela rota
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            try
            {
                RoleReturnVO user = await _roleService.GetByIdAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Perfil listado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar perfil pelo id {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar perfil pelo id {id}"));
            }
        }

        /// <summary>
        /// GetAll - Método para listar perfis, sem parâmetros
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<RoleReturnVO> user = await _roleService.GetAllAsync();
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Perfis listados com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar perfis",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar perfis"));
            }
        }
    }
}
