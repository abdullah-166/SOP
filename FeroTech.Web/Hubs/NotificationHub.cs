using Microsoft.AspNetCore.SignalR;

namespace FeroTech.Web.Hubs
{
    public class NotificationHub : Hub
    {
        // This class is required to exist so we can inject IHubContext<NotificationHub>
        // into your controllers.
    }
}