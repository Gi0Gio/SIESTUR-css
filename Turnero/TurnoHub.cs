using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class TurnoHub : Hub
{
    public async Task NotifyTurnUpdated()
    {
        await Clients.All.SendAsync("TurnUpdated");
    }
}