using Microsoft.AspNetCore.SignalR;

namespace SportsDay.Web.Hubs;

public class SportsHub : Hub
{
    public async Task SendUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveUpdate", message);
    }

    public async Task SendAnnouncement(string message, string priority)
    {
        await Clients.All.SendAsync("ReceiveAnnouncement", message, priority);
    }

    public async Task SendResult(string eventName, string participantName, string result)
    {
        await Clients.All.SendAsync("ReceiveResult", eventName, participantName, result);
    }

    public async Task UpdateLeaderboard()
    {
        await Clients.All.SendAsync("RefreshLeaderboard");
    }

    public async Task UpdateEventStatus(string eventName, string status)
    {
        await Clients.All.SendAsync("ReceiveEventStatus", eventName, status);
    }
}
