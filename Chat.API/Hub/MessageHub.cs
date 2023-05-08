using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hub
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        public async Task SendMessagesToAllUsersAsync(List<string> messages)
        {
            await Clients.All.SendMessagesToAllUsersAsync(messages);
        }
    }
}
