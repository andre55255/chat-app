namespace Chat.API.Hub
{
    public interface IMessageHubClient
    {
        Task SendMessagesToAllUsersAsync(List<string> messages);
    }
}
