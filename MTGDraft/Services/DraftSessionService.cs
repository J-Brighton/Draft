using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Player;

public class DraftSessionService
{
    private readonly DraftContext _context;

    public DraftSessionService(DraftContext context)
    {
        _context = context;
    }

    public async Task<DraftSession> CreateDraftSession(string setCode, int playerCount)
    {
        // get the set from database
        var set = await _context.Sets
                    .Include(set => set.Cards)
                    .FirstOrDefaultAsync(set => set.Code == setCode);

        if (set == null) throw new ArgumentException("invalid set code");
        
        Console.WriteLine($"Set loaded: {set.Code}, Cards count: {set.Cards.Count}");

        // use the factory to create the draft session & packs
        var session = DraftSessionFactory.Create(set, playerCount);

        // save to database
        _context.DraftSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<DraftSession> JoinDraftSession(int sessionId, int playerId)
    {
        // find the session
        var session = await _context.DraftSessions
            .Include(session => session.DraftPlayers)
            .FirstOrDefaultAsync(session => session.Id == sessionId);

        // error if not found
        if (session == null) throw new ArgumentException("invalid session id");

        // check its not full
        if (session.DraftPlayers.Count >= session.PlayerCount)
        {
            throw new InvalidOperationException("session is full");
        }

        // get the player
        var player = await _context.Players.FindAsync(playerId);
        if (player == null) throw new ArgumentException("invalid player id");

        // check player isn't already in the session
        if (session.DraftPlayers.Any(p => p.Id == playerId))
        {
            throw new InvalidOperationException("player already in session");
        }

        session.DraftPlayers.Add(player);
        await _context.SaveChangesAsync();

        // return the updated session
        return session;
    }

    // public async Task<DraftSession> StartDraftSession()
    // {
        
        // find the session

        // check it exists, isn't already started, has 8 players

        // init draft state (has started, current pack number, current pick number, passing left or right, is complete)

        // assign a seat to each player

        // run the pack generation

        // assign 3 packs to each player based on seat

        // save and return the updated session

    // }
}