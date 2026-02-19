namespace MTGDraft.Models;

public class DraftSession
{
    public int Id { get; set; }

    public string SetCode { get ; set; } = null!;

    public DateTime CreatedAt { get; set; }


    public ICollection<Pack> Packs { get; set; } = new List<Pack>();

}