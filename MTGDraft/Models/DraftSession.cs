using MTGDraft.DTOs.Player;

namespace MTGDraft.Models;

public class DraftSession
{
    public int Id { get; set; }
    public string SetCode { get ; set; } = null!;
    public int PlayerCount { get; set; }
    public List<Player> DraftPlayers { get; set; } = new List<Player>();
    public string DraftState { get; set;} = null!;
    public int CurrentPackIndex { get; set; }
    public int CurrentPickIndex { get; set; }
    public bool DraftDirectionClockwise { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Pack> Packs { get; set; } = new List<Pack>();


    public void AddPlayer(Player player)
    {
        // check player count
        if (DraftPlayers.Count >= PlayerCount)
        {
            throw new InvalidOperationException("session is full");
        }

        // check if already in session
        if (DraftPlayers.Any(p => p.Id == player.Id))
        {
            throw new InvalidOperationException("player already in session");
        }

        // check if the session already started
        if (DraftState != "NotStarted")
        {
            throw new InvalidOperationException("cannot join a started draft");
        }

        // add em
        DraftPlayers.Add(player);
    }

    public void StartDraft(Set set)
    {
        // check for player count
        if (DraftPlayers.Count != PlayerCount)
        {
            throw new InvalidOperationException($"Cannot start draft without exactly {PlayerCount} players");
        }

        // check if already started
        if (DraftState != "NotStarted")
        {
            throw new InvalidOperationException("Draft has already started");
        }

        // generate packs
        Packs = DraftSessionFactory.GeneratePacks(set, PlayerCount);

        // initialise the draft
        DraftState = "InProgress";
        CurrentPackIndex = 0;
        CurrentPickIndex = 0;
        DraftDirectionClockwise = true;
    }

    public void PickCard(int playerId, int packCardId)
    {
        // check if draft is running
        if (DraftState != "InProgress") throw new InvalidOperationException("draft is not in progress");

        // check if player exists
        var player = DraftPlayers.FirstOrDefault(p => p.Id == playerId);
        if (player == null) throw new ArgumentException("invalid player id");

        // check if pack contains the selected card
        var pack = Packs.FirstOrDefault(p => p.Cards.Any(c => c.Id == packCardId));
        if (pack == null) throw new ArgumentException("invalid pack");

        // card isn't already picked
        var packCard = pack.Cards.First(c => c.Id == packCardId);
        if (packCard.IsPicked) throw new InvalidOperationException("card already picked");

        // pick it
        packCard.IsPicked = true;
        packCard.PickedByPlayerId = playerId;
    }

}