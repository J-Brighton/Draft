using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;

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
}