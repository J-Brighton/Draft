using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using MTGDraft.Hubs;
using MTGDraft.Enums;


public class DraftTimerService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DraftNotificationService _notifications;
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _timers = new();

    public DraftTimerService(IServiceScopeFactory scopeFactory, DraftNotificationService notifications)
    {
        _scopeFactory = scopeFactory;
        _notifications = notifications;
    }

    public void CancelDraft(int sessionId)
    {
        if (_timers.TryRemove(sessionId, out var existing)) existing.Cancel();
    }

    public async Task StartTimer(int sessionId, DateTime deadline)
    {
        CancelDraft(sessionId);

        var cts = new CancellationTokenSource();
        _timers[sessionId] = cts;

        _ = RunTimer(sessionId, deadline, cts.Token);

        await _notifications.BroadcastTimerStart(sessionId, deadline);
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
                await _notifications.BroadcastTimeLeft(sessionId, i);
                await Task.Delay(1000, token);
            }

            await OnTimerExpired(sessionId);

        } catch (TaskCanceledException)
        {
            await _notifications.BroadcastTimerStopped(sessionId);
        }
    }

    private async Task OnTimerExpired(int sessionId)
    {
        using var scope = _scopeFactory.CreateScope();

        var pickService = scope.ServiceProvider.GetRequiredService<DraftPickService>();
        var flowService = scope.ServiceProvider.GetRequiredService<DraftFlowService>();

        await pickService.AutoPickCard(sessionId);
        await pickService.BotPickCard(sessionId);

        var session = await flowService.Advance(sessionId);

        if (session.DraftState != DraftState.Complete)
        {
            var newDeadline = DateTime.UtcNow.AddSeconds(60);
            await StartTimer(sessionId, newDeadline);
        } else
        {
            await _notifications.BroadcastDraftComplete(sessionId);
        }
    }
} 



