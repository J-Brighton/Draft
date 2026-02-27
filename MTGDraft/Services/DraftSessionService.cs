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

        var player = await _context.Players.FindAsync(playerId);
        if (player == null) throw new ArgumentException("invalid player id");
    
        session.AddPlayer(player);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<DraftSession> StartDraftSession(int sessionId)
    {
        var session = await _context.DraftSessions
            .Include(s => s.DraftPlayers)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        // check the session is real
        if (session == null) throw new ArgumentException("invalid session id"); 

        // find the set from code
        var draftSet = await _context.Sets.FirstOrDefaultAsync(s => s.Code == session.SetCode);
        if (draftSet == null) throw new ArgumentException("invalid set code");
        
        // start the draft
        session.StartDraft(draftSet);
        await _context.SaveChangesAsync();

        return session;
    }
}