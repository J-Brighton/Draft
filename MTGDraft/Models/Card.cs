namespace MTGDraft.Models;

public class Card
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Rarity { get; set; } = "";

    public string SetCode { get; set; } = "";
    
    public int CardNumber { get; set; }

    // foreign key linking this card to its set
    public int SetId { get; set; }

    // navigation property for EF Core
    public Set? Set { get; set; }
}

