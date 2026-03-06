using MTGDraft.Enums;
using MTGDraft.Models;

namespace MTGDraft.PackGeneration;

public class SetCardBuckets
{
    public List<Card> Common { get; set; } = [];
    public List<Card> Uncommon { get; set; } = [];
    public List<Card> Rare { get; set; } = [];
    public List<Card> Mythic { get; set; } = [];

    public List<Card> FableUncommon { get; set; } = [];
    public List<Card> FableRare { get; set; } = [];
    public List<Card> FableMythic { get; set; } = [];

    public List<Card> BorderlessRare { get; set; } = [];
    public List<Card> BorderlessMythic { get; set; } = [];

    public List<Card> ReversableShock { get; set; } = [];

    public List<Card> BasicLands { get; set; } = [];
    public List<Card> FullArtBasicLands { get; set; } = [];

    public List<Card> SpecialGuests { get; set; } = [];

    private static List<Card> GetBucket(
        Dictionary<(CardTreatment Treatment, string Rarity), List<Card>> groups,
        CardTreatment treatment,
        string rarity)
    {
        return groups.TryGetValue((treatment, rarity), out var list) 
            ? list
            : new List<Card>();
    } 

    public static SetCardBuckets Build(Set set)
    {
        var groups = set.Cards.GroupBy(c => (c.Treatment, c.Rarity)).ToDictionary(g => g.Key, g => g.ToList());

        return new SetCardBuckets
        {
            Common = GetBucket(groups, CardTreatment.Regular, "C").Where(c => c.IsBasicLand == false).ToList(),
            Uncommon = GetBucket(groups, CardTreatment.Regular, "U"),
            Rare = GetBucket(groups, CardTreatment.Regular, "R"),
            Mythic = GetBucket(groups, CardTreatment.Regular, "M"),

            FableUncommon = GetBucket(groups, CardTreatment.Fable, "U"),
            FableRare = GetBucket(groups, CardTreatment.Fable, "R"),
            FableMythic = GetBucket(groups, CardTreatment.Fable, "M"),

            BorderlessRare = GetBucket(groups, CardTreatment.Borderless, "R"),
            BorderlessMythic = GetBucket(groups, CardTreatment.Borderless, "M"),

            ReversableShock = GetBucket(groups, CardTreatment.ReversibleShock, "R"),

            BasicLands = GetBucket(groups, CardTreatment.Regular, "C").Where(c => c.IsBasicLand).ToList(),
            FullArtBasicLands = GetBucket(groups, CardTreatment.FullArt, "C").Where(c => c.IsBasicLand).ToList(),

            SpecialGuests = groups.Where(kvp => kvp.Key.Treatment == CardTreatment.SpecialGuest).SelectMany(kvp => kvp.Value).ToList()  
        };
    }
}