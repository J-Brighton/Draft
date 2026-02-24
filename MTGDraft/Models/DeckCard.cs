namespace MTGDraft.Models;

public class DeckCard
{
    public int Id { get; set; }
    public int DeckId { get; set; }
    public int CardId { get; set; }

    public int Quantity { get; set; }

    public bool IsSideboard { get; set; }

    public Deck Deck { get; set; } = null!;
    public Card Card { get; set; } = null!;
}
