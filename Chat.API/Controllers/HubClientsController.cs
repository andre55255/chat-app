using Chat.API.Hub;
using Chat.Communication.ViewObjects.APIResponse;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("/Hub")]
    public class HubClientsController : ControllerBase
    {
        private readonly IHubContext<MessageHub, IMessageHubClient> _messagesHub;
        private readonly ILogService _logService;

        public HubClientsController(IHubContext<MessageHub, IMessageHubClient> messagesHub, ILogService logService)
        {
            _messagesHub = messagesHub;
            _logService = logService;
        }

        /// <summary>
        /// MessagesAllClientsToString - Método para envio de mensagens como string para todos os clientes conectados ao hub, passar dados pelo body
        /// </summary>
        [HttpPost]
        [Route("/MessagesAllClientsToString")]
        public async Task<IActionResult> MessagesAllClientsToString([FromBody] List<string> messages)
        {
            try
            {
                await _messagesHub.Clients.All.SendMessagesToAllUsersAsync(messages);

                return StatusCode(StatusCodes.Status200OK,
                    APIResponseVO.Ok($"Mensagens disparadas com sucesso para todos os usuários do hub"));
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex,
                    $"Falha inesperada ao executar rotina de disparo de mensagens para todos os clientes do hub: {StaticMethods.SerializeObject(messages)}",
                    this.GetType().ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    APIResponseVO.Fail($"Falha inesperada ao executar rotina de disparo de mensagens para todos os clientes do hub"));
            }
        }
    }
}
