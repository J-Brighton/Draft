using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;

public class DraftEngineService
{
    private readonly DraftContext _context;

    public DraftEngineService(DraftContext context)
    {
        _context = context;
    }

    // PICK CARD
    // ADVANCE ROUND
    // AI PICKS
    // PASS PACKS

    public async Task<DraftSession> PickCard(int sessionId, int playerId, int packCardId)
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
        session.PickCard(playerId, packCardId);

        // save changes
        await _context.SaveChangesAsync();
        return session;
    }
}