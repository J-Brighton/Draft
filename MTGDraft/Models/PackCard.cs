namespace MTGDraft.Models;

public class PackCard
{
    public int Id { get ; set; }

    public int PackId { get; set; }
    public int CardId { get; set; }
    
    public bool IsPicked { get; set; }
    public int? PickedByPlayerId { get; set; }
    public bool IsFoil { get; set; } = false;
    
    public Pack Pack { get; set; } = null!;
    public Card Card { get; set; } = null!;
}