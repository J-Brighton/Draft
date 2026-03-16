using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Features.Draft.DTOs;
using MTGDraft.Models;

public class DraftPickService
{
    private readonly DraftContext _context;
    private readonly DraftNotificationService _notifications;

    public DraftPickService(DraftContext context, DraftNotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task<PackCard> PickCard(int sessionId, PickPackCardDTO pick)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .Include(s => s.Packs)
                .ThenInclude(p => p.Cards)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session Id");
        
        var pickedCard = session.PickCard(pick);
        await _context.SaveChangesAsync();

        await _notifications.BroadcastPlayerPick(sessionId, pick.PlayerId);
        return pickedCard;
    }

    public async Task AutoPickCard(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .Include(s => s.Packs)
                .ThenInclude(p => p.Cards)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session Id");

        foreach (var player in session.DraftPlayers.Where(p => !p.IsBot && !p.HasPickedThisRound))
        {
            var pack = session.Packs.FirstOrDefault(
                p => p.CurrentSeat == player.DraftSessionSeat &&
                    p.PackNumber == session.CurrentPackNumber &&
                    p.Cards.Any(c => !c.IsPicked)
            );
            if (pack == null) continue;

            var cardToPick = pack.Cards
                .Where(c => !c.IsPicked)
                .OrderBy(_ => Guid.NewGuid())
                .First();

            session.PickCard(new PickPackCardDTO(player.Id, cardToPick.Id));
            await _notifications.BroadcastPlayerPick(sessionId, player.Id);
        }

        await _context.SaveChangesAsync();
    }

    public async Task BotPickCard(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .Include(s => s.Packs)
                .ThenInclude(p => p.Cards)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session Id");

        foreach (var bot in session.DraftPlayers.Where(p => p.IsBot && !p.HasPickedThisRound))
        {
            var pack = session.Packs.FirstOrDefault(
                p => p.CurrentSeat == bot.DraftSessionSeat &&
                    p.PackNumber == session.CurrentPackNumber &&
                    p.Cards.Any(c => !c.IsPicked)
            );
            if (pack == null) continue;

            var cardToPick = pack.Cards
                .Where(c => !c.IsPicked)
                .OrderBy(_ => Guid.NewGuid())
                .First();

            session.PickCard(new PickPackCardDTO(bot.Id, cardToPick.Id));
            await _notifications.BroadcastPlayerPick(sessionId, bot.Id);
        }

        await _context.SaveChangesAsync();
    }

}
