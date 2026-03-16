using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Enums;
using MTGDraft.Models;

public class DraftFlowService
{
    private readonly DraftContext _context;

    public DraftFlowService(DraftContext context)
    {
        _context = context;
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