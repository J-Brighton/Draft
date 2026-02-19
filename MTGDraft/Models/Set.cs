namespace MTGDraft.Models;

public class Set
{
    public int Id { get ; set; }
    public string Code { get ; set; } = null!;
    public string Name { get ; set ; } = null!;

    public ICollection<Card> Cards { get; set; } = new List<Card>();
}