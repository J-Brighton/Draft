namespace MTGDraft.Models;

public class Deck
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public Player? Player { get; set; }

    public List<DeckCard> DeckCards { get; set; } = new List<DeckCard>();    
}