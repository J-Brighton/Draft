namespace MTGDraft.Models;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<Deck> Decks { get; set; } = new List<Deck>();
}