using MTGDraft.DTOs.Player;

namespace MTGDraft.Models;

public class DraftSession
{
    public int Id { get; set; }
    public string SetCode { get ; set; } = null!;
    public int PlayerCount { get; set; }
    public List<Player> DraftPlayers { get; set; } = new List<Player>();
    public string DraftState { get; set;} = null!;

    public DateTime CreatedAt { get; set; }
    public List<Pack> Packs { get; set; } = new List<Pack>();

}