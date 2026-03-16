using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.Features.Draft.PackGeneration;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.CodeAnalysis;

public class DraftSessionService
{
    private readonly DraftContext _context;
    private readonly DraftTimerService _timer;

    public DraftSessionService(DraftContext context, DraftTimerService timer)
    {
        _context = context;
        _timer = timer;
    }

    public async Task<DraftSession> CreateDraftSession(string setCode, int playerCount)
    {
        // get the set from database
        var set = await _context.Sets
                    .Include(set => set.Cards)
                    .FirstOrDefaultAsync(set => set.Code == setCode);

        if (set == null) throw new ArgumentException("invalid set code");
        
        // use the factory to create the draft session & packs
        var session = DraftSessionFactory.Create(set, playerCount);

        // save to database
        _context.DraftSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    /// <summary>
    /// Joins a draft session by creating a player entity that represents the given user within the context of the specific draft session
    /// </summary>
    /// <param name="sessionId">Id of draft session to join</param>
    /// <param name="userId">Id of the authenticated user joining the draft</param>
    /// <returns>updated draft session</returns>
    /// <exception cref="ArgumentException">when the session or user doesn't exist</exception>
    public async Task<DraftSession> JoinDraftSession(int sessionId, int userId)
    {
        var session = await _context.DraftSessions
            .Include(session => session.DraftPlayers)
            .FirstOrDefaultAsync(session => session.Id == sessionId);
        if (session == null) throw new ArgumentException("invalid session id");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("invalid user id");

        if (session.DraftPlayers.Any(p => p.UserId == userId)) throw new ArgumentException("user already joined this draft session");

        var player = new Player
        {
            UserId = userId,
            Name = user.Username,
            IsBot = false,
            DraftSessionId = sessionId
        };

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

        // find the set from code -> include cards for pack generation in StartDraft function
        var draftSet = await _context.Sets.Include(s => s.Cards).FirstOrDefaultAsync(s => s.Code == session.SetCode);
        if (draftSet == null) throw new ArgumentException("invalid set code");
        
        if (session.DraftPlayers.Count < session.PlayerCount)
        {
            session.PopulateSession(session);
        }

        // generate the packs
        var generator = new PackGenerator();
        var packs = generator.GeneratePacks(draftSet, session.PlayerCount);

        // start the draft
        session.StartDraft(packs);
        await _context.SaveChangesAsync();
        await _timer.StartTimer(sessionId, DateTime.UtcNow.AddSeconds(60));

        return session;
    }
}