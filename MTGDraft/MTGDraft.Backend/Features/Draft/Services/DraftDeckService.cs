using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Enums;
using MTGDraft.Models;

public class DraftDeckService
{
    private readonly DraftContext _context;

    public DraftDeckService(DraftContext context)
    {
        _context = context;
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

}



    
