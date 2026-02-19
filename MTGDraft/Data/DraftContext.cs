using Microsoft.EntityFrameworkCore;
using MTGDraft.Models;

namespace MTGDraft.Data;

public class DraftContext(DbContextOptions<DraftContext> options) : DbContext(options)
{
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Pack> Packs => Set<Pack>();
    public DbSet<DraftSession> DraftSessions => Set<DraftSession>();
    public DbSet<PackCard> PackCards => Set<PackCard>();
    public DbSet<Set> Sets => Set<Set>();
}
