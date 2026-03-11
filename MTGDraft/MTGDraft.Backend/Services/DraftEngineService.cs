using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.PackCard;
using MTGDraft.Enums;
using MTGDraft.Models;
using MTGDraft.Hubs;

public class DraftEngineService
{
    private readonly DraftContext _context;
    private readonly DraftTimerService _timer;
    private readonly IHubContext<DraftHub> _hub;

    public DraftEngineService(DraftContext context, DraftTimerService timer, IHubContext<DraftHub> hub)
    {
        _context = context;
        _timer = timer;
        _hub = hub;
    }

    public async Task StartPickTimer(int sessionId)
    {
        var deadline = DateTime.UtcNow.AddSeconds(20);
        var session = await _context.DraftSessions.FindAsync(sessionId);
        if (session == null) throw new ArgumentException("invalid session id");

        session.PickDeadline = deadline;
        await _context.SaveChangesAsync();

        await _timer.ScheduleSession(sessionId, deadline);
        await _timer.BroadcastTimerStart(sessionId, deadline);

    }

    public async Task StopPickTimer(int sessionId)
    {
        var session = await _context.DraftSessions.FindAsync(sessionId);
        if (session == null) throw new ArgumentException("invalid session id");

        session.PickDeadline = null;
        await _context.SaveChangesAsync();

        _timer.CancelDraft(sessionId);
    }

    public async Task<DraftSession> Advance(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(session => session.DraftPlayers)
            .Include(session => session.Packs)
                .ThenInclude(pack => pack.Cards)
            .FirstOrDefaultAsync(session => session.Id == sessionId);

        if (session == null) throw new ArgumentException("invalid session id");

        session.Advance();
        await _context.SaveChangesAsync();

        if (session.DraftState == DraftState.Complete)
        {
            await _hub.Clients.Group($"draft-{sessionId}").SendAsync("DraftComplete");
            await StopPickTimer(sessionId);
            await CreateDraftDecks(sessionId);
        }
        return session;
    }

    public async Task<PackCard> PickCard(int sessionId, PickPackCardDTO pick)
    {
        // find the session
        var session = await _context.DraftSessions
            .Include(session => session.DraftPlayers)
            .Include(session => session.Packs)
                .ThenInclude(pack => pack.Cards)
            .FirstOrDefaultAsync(session => session.Id == sessionId);
        
        // error if no session
        if (session == null) throw new ArgumentException("invalid session id");

        // pick the card
        var pickedCard = session.PickCard(pick);
        await _context.SaveChangesAsync();

        await BroadcastPlayerPick(sessionId, pick.PlayerId);
        return pickedCard;
    }

    public async Task AutoPickCard(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .Include(s => s.Packs)
                .ThenInclude(p => p.Cards)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session id");

        foreach (var player in session.DraftPlayers.Where(p => !p.IsBot && !p.HasPickedThisRound))
        {
            var pack = session.Packs.FirstOrDefault(
                p => p.CurrentSeat == player.DraftSessionSeat && 
                p.PackNumber == session.CurrentPackNumber && 
                p.Cards.Any(c => !c.IsPicked)
            );
            if (pack == null) continue;

            // pick random
            var cardToPick = pack.Cards
                .Where(c => !c.IsPicked)
                .OrderBy(_ => Guid.NewGuid())
                .First();

            session.PickCard(new PickPackCardDTO(player.Id, cardToPick.Id));
            await BroadcastPlayerPick(sessionId, player.Id);
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
            
        if (session == null) throw new ArgumentException("invalid session id");

        foreach (var bot in session.DraftPlayers.Where(p => p.IsBot && !p.HasPickedThisRound))
        {
            var pack = session.Packs.FirstOrDefault(
                p => p.CurrentSeat == bot.DraftSessionSeat && 
                p.PackNumber == session.CurrentPackNumber && 
                p.Cards.Any(c => !c.IsPicked)
            );
            if (pack == null) continue;

            // pick random
            var cardToPick = pack.Cards
                .Where(c => !c.IsPicked)
                .OrderBy(_ => Guid.NewGuid())
                .First();

            session.PickCard(new PickPackCardDTO(bot.Id, cardToPick.Id));
            await BroadcastPlayerPick(sessionId, bot.Id);

        }

        await _context.SaveChangesAsync();
    }

    public async Task CreateDraftDecks(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .Include(s => s.Packs)
                .ThenInclude(p => p.Cards)
                    .ThenInclude(pc => pc.Card)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session id");
        if (session.DraftState != DraftState.Complete) throw new InvalidOperationException("draft not complete");

        foreach (var player in session.DraftPlayers)
        {
            var deck = new Deck
            {
                PlayerId = player.Id,
                Name = $"{session.SetCode} - {sessionId} Draft Deck"
            };

            var draftedCards = session.Packs
                .SelectMany(p => p.Cards)
                .Where(c => c.PickedByPlayerId == player.Id);

            foreach (var card in draftedCards)
            {
                deck.DeckCards.Add(new DeckCard
                {
                   CardId = card.Card.Id 
                });
            }
            
            _context.Decks.Add(deck);
        }

        await _context.SaveChangesAsync();
    }

    public async Task ClearSessionId(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    
        if (session == null) throw new ArgumentException("invalid session id");

        foreach (var player in session.DraftPlayers)
        {
            player.DraftSessionId = null;
            player.DraftSessionSeat = null;
        }

        await _context.SaveChangesAsync();
    }

    public async Task BroadcastPlayerPick(int sessionId, int playerId)
    {
        await _hub.Clients.Group($"draft-{sessionId}").SendAsync("PlayerPicked", playerId);
    }
}