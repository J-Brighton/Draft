using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.PackCard;
using MTGDraft.Migrations;
using MTGDraft.Models;

public class DraftEngineService
{
    private readonly DraftContext _context;

    public DraftEngineService(DraftContext context)
    {
        _context = context;
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

        // save changes
        await _context.SaveChangesAsync();
        return pickedCard;
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
        return session;
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

        if (session.DraftState != "Completed") throw new InvalidOperationException("draft not complete");

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

}