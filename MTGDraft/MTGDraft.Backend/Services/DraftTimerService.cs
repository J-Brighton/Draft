using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using MTGDraft.Hubs;

public class DraftTimerService(IServiceScopeFactory scopeFactory, IHubContext<DraftHub> hub)
{
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _timers = new();

    public async Task ScheduleSession(int sessionId, DateTime deadline)
    {
        CancelDraft(sessionId);

        var cts = new CancellationTokenSource();
        _timers[sessionId] = cts;

        await RunTimer(sessionId, deadline, cts.Token);
    }

    public void CancelDraft(int sessionId)
    {
        if (_timers.TryRemove(sessionId, out var existing))
        {
            existing.Cancel();
        }
    }

    private async Task RunTimer(int sessionId, DateTime deadline, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var timeLeft = deadline - now;
        var totalSeconds = Math.Max(0, (int)Math.Ceiling(timeLeft.TotalSeconds));

        try
        {
            for (int i = totalSeconds ; i > 0 ; i--)
            {
                await hub.Clients.Group($"draft-{sessionId}").SendAsync("TimeLeft", i);
                await Task.Delay(1000, token);
            }

            using var scope = scopeFactory.CreateScope();
            var engine = scope.ServiceProvider.GetRequiredService<DraftEngineService>();

            await engine.AutoPickCard(sessionId);
            await engine.BotPickCard(sessionId);
            await engine.Advance(sessionId);
            await ScheduleSession(sessionId, DateTime.UtcNow.AddSeconds(20));

        } catch (TaskCanceledException)
        {
            await hub.Clients.Group($"draft-{sessionId}").SendAsync("Timer Ended");
        }
    }

    public async Task BroadcastTimerStart(int sessionId, DateTime deadline)
    {
        await hub.Clients.Group($"draft-{sessionId}").SendAsync("TimerStarted", new { deadline });
    }
}