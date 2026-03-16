using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MTGDraft.Hubs;

public class DraftNotificationService
{
    private readonly IHubContext<DraftHub> _hub;

    public DraftNotificationService(IHubContext<DraftHub> hub)
    {
        _hub = hub;
    }

    public Task BroadcastPlayerPick(int sessionId, int playerId)
    {
        return _hub.Clients
            .Group($"draft-{sessionId}")
            .SendAsync("PlayerPicked", playerId);
    }

    public Task BroadcastDraftComplete(int sessionId)
    {
        return _hub.Clients
            .Group($"draft-{sessionId}")
            .SendAsync("DraftComplete");
    }

    public Task BroadcastTimerStart(int sessionId, DateTime deadline)
    {
        return _hub.Clients
            .Group($"draft-{sessionId}")
            .SendAsync("TimerStarted", deadline);
    }

    public Task BroadcastTimeLeft(int sessionId, int seconds)
    {
        return _hub.Clients
            .Group($"draft-{sessionId}")
            .SendAsync("TimeLeft", seconds);
    }

    public Task BroadcastTimerStopped(int sessionId)
    {
        return _hub.Clients
            .Group($"draft-{sessionId}")
            .SendAsync("TimerStopped");
    }
}





    