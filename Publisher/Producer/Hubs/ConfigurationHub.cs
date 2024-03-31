using Microsoft.AspNetCore.SignalR;

namespace Producer.Hubs
{
    public class ConfigurationHub : Hub
    {
        public async Task SendNotification(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("Notification", message);
        }
    }
}
