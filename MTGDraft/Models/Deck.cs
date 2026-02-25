namespace MTGDraft.Models;

public class Deck
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int PlayerId { get; set; }

    public Player Player { get; set; } = null!;

    public List<DeckCard> DeckCards { get; set; } = new List<DeckCard>();    
}