using MTGDraft.DTOs.PackCard;
using MTGDraft.DTOs.Player;
using MTGDraft.Migrations;

namespace MTGDraft.Models;

public class DraftSession
{
    public int Id { get; set; }
    public string SetCode { get ; set; } = null!;
    public int PlayerCount { get; set; }
    public List<Player> DraftPlayers { get; set; } = new List<Player>();
    public string DraftState { get; set;} = null!;
    public int CurrentPackNumber { get; set; } = 1;
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
        player.DraftSessionSeat = DraftPlayers.Count();
    }

    public void StartDraft(Set set)
    {
        // check for player count
        if (DraftPlayers.Count != PlayerCount) throw new InvalidOperationException($"Cannot start draft without exactly {PlayerCount} players");

        // check if already started
        if (DraftState != "NotStarted") throw new InvalidOperationException("Draft has already started");

        Console.WriteLine("BEFORE PACK GENERATION");
        // generate packs
        Packs = DraftSessionFactory.GeneratePacks(set, PlayerCount);

        // initialise the draft
        DraftState = "InProgress";
        CurrentPackNumber = 1;
        CurrentPickIndex = 0;
        DraftDirectionClockwise = true;
    }

    public PackCard PickCard(PickPackCardDTO pick)
    {
        // check if draft is running
        if (DraftState != "InProgress") throw new InvalidOperationException("draft is not in progress");

        // check if player exists
        var player = DraftPlayers.FirstOrDefault(p => p.Id == pick.PlayerId);
        if (player == null) throw new ArgumentException("invalid player id");

        // check player hasn't already picked
        if (player.HasPickedThisRound) throw new InvalidOperationException("player has already picked this round");

        // check if it is players pack
        var playerPack = Packs.FirstOrDefault(p => p.CurrentSeat == player.DraftSessionSeat);
        if (playerPack == null) throw new InvalidOperationException("no pack at player seat");

        // check if pack contains the selected card
        var packCard = playerPack.Cards.FirstOrDefault(c => c.Id == pick.PackCardId);
        if (packCard == null) throw new InvalidOperationException("card doesn't belong to player pack");

        if (packCard.IsPicked) throw new InvalidOperationException("card already picked");

        // pick it
        packCard.IsPicked = true;
        packCard.PickedByPlayerId = pick.PlayerId;
        player.HasPickedThisRound = true;

        return packCard;
    }

    public void Advance()
    {
        // make sure game is in progress
        if (DraftState != "InProgress") throw new InvalidOperationException("draft is not in progress");

        // make sure all players have picked
        if (!DraftPlayers.All(p => p.HasPickedThisRound)) throw new InvalidOperationException("not all players have picked");

        // pass packs around
        foreach (var pack in Packs)
        {
            if (pack.Cards.Any(c => !c.IsPicked))
            {
                if (DraftDirectionClockwise)
                {
                    pack.CurrentSeat = (pack.CurrentSeat % PlayerCount) + 1;
                } else
                {
                    pack.CurrentSeat = (pack.CurrentSeat - 2 + PlayerCount) % PlayerCount + 1;
                }
            }
        }

        // reset pick bool
        foreach (var player in DraftPlayers)
        {
            player.HasPickedThisRound = false;
        }

        CurrentPickIndex++;

        bool currentRoundFinished = Packs
            .Where(p => p.PackNumber == CurrentPackNumber)
            .All(p => p.Cards.All(c => c.IsPicked));

        if (currentRoundFinished)
        {
            CurrentPackNumber++;
            DraftDirectionClockwise = !DraftDirectionClockwise;
            CurrentPickIndex = 0;
        }

        if (CurrentPackNumber > 3)
        {
            DraftState = "Completed";
        }
    }
}