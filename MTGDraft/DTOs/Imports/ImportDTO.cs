public class SetImportDTO
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<CardImportDTO> Cards { get; set; } = new();
}

public class CardImportDTO
{
    public string Name { get; set; } = null!;
    public string Rarity { get; set; } = null!;
    public int CardNumber { get; set; }
}