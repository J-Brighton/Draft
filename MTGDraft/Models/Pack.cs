namespace MTGDraft.Models;

public class Pack
{
    public int Id { get ; set; }
    public int DraftSessionId { get; set; }
    public int PackNumber { get; set; }
    public int OriginalSeat { get; set; }

    public DraftSession DraftSession { get; set; } = null!;

    public ICollection<PackCard> Cards { get; set; } = new List<PackCard>();
}