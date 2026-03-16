using MTGDraft.Enums;

namespace MTGDraft.Models;

public class Card
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Rarity { get; set; } = "";

    public string SetCode { get; set; } = "";
    
    public int CardNumber { get; set; }

    public int SetId { get; set; }
    public Set? Set { get; set; }

    public CardTreatment Treatment { get; set; } = CardTreatment.Regular;
    public FoilType FoilType { get; set; } = FoilType.NonFoil;
    public bool IsBasicLand { get; set; }
}

