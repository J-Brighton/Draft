using MTGDraft.Enums;

public class CardImportDTO
{
    public string Name { get; set; } = null!;
    public string Rarity { get; set; } = null!;
    public int CardNumber { get; set; }
    public CardTreatment Treatment { get; set; }
    public FoilType FoilType { get; set; }
    public bool IsBasicLand { get; set; }
}