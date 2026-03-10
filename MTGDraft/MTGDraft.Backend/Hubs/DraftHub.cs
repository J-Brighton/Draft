using Microsoft.AspNetCore.SignalR;

namespace MTGDraft.Hubs;

public class DraftHub : Hub
{
    public async Task JoinDraft(int sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"joined draft-{sessionId}");
    }

    public async Task LeaveDraft(int sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"left draft-{sessionId}");
    }
}