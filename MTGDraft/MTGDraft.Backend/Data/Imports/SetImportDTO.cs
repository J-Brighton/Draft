using MTGDraft.Enums;

public class SetImportDTO
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<CardImportDTO> Cards { get; set; } = new();
}