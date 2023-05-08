using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Communication.ViewObjects.EndPoints;
using Chat.Communication.ViewObjects.Utils;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndPointController : ControllerBase
    {
        private readonly IEndPointService _endpointService;
        private readonly ILogService _logService;

        public EndPointController(IEndPointService endpointService, ILogService logService)
        {
            _endpointService = endpointService;
            _logService = logService;
        }

        /// <summary>
        /// Create - Método para criar endpoint no sistema, passar dados pelo body
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] EndPointSaveVO model)
        {
            try
            {
                EndPointSaveVO response = await _endpointService.CreateAsync(model);
                return StatusCode(StatusCodes.Status201Created,
                    APIResponseVO.Ok($"Endpoint criado com sucesso", response));
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
                    $"Falha inesperada ao executar rotina de criação de endpoint: {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de criação de endpoint"));
            }
        }

        /// <summary>
        /// Edit - Método para editar endpoint no sistema, passar id pela rota e dados de endpoint pelo body
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] EndPointSaveVO model)
        {
            try
            {
                model.Id = id;
                EndPointSaveVO response = await _endpointService.EditAsync(model);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Endpoint editado com sucesso", response));
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
                    $"Falha inesperada ao executar rotina de edição de endpoint: {id} - {StaticMethods.SerializeObject(model)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de edição de endpoint"));
            }
        }

        /// <summary>
        /// Delete - Método para deletar endpoint no sistema, passar id pela rota
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            try
            {
                EndPointSaveVO response = await _endpointService.DeleteAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Endpoint deletado com sucesso", response));
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
                    $"Falha inesperada ao executar rotina de deleção de endpoint {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de deleção de endpoint"));
            }
        }

        /// <summary>
        /// GetById - Método para lisatr endpoint pelo id, passar id pela rota
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            try
            {
                EndPointSaveVO user = await _endpointService.GetByIdAsync(id);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Endpoint listado com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar endpoint pelo id {id}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar endpoint pelo id {id}"));
            }
        }

        /// <summary>
        /// GetAll - Método para listar endpoints filtrados, passar filtro opcional pela query
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterEndpointVO filter)
        {
            try
            {
                ListAllEntityVO<EndPointSaveVO> user = await _endpointService.GetAllAsync(filter);
                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Endpoints listados com sucesso", user));
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
                    $"Falha inesperada ao executar rotina de listar endpoints: {StaticMethods.SerializeObject(filter)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de listar endpoints"));
            }
        }
    }
}
