using Microsoft.AspNetCore.SignalR;

namespace SportsDay.Web.Hubs;

public class SportsHub : Hub
{
    public async Task SendEventUpdate(string eventName, string status)
    {
        await Clients.All.SendAsync("ReceiveEventUpdate", eventName, status);
    }

    public async Task SendResult(string eventName, string participantName, string result)
    {
        await Clients.All.SendAsync("ReceiveResult", eventName, participantName, result);
    }

    public async Task SendAnnouncement()
    {
        await Clients.All.SendAsync("ReceiveAnnouncement");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
