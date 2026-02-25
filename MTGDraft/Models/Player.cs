namespace MTGDraft.Models;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool IsBot { get; set; }

    public int? DraftSessionId { get; set; }
    public DraftSession? DraftSession { get; set; }

    public List<Deck> Decks { get; set; } = new List<Deck>();
}