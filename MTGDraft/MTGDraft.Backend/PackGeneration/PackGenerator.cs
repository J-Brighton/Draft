using Microsoft.AspNetCore.Mvc.Filters;
using MTGDraft.Models;
using SQLitePCL;

namespace MTGDraft.PackGeneration;

public interface IPackGenerator
{
    List<Pack> GeneratePacks(Set set, int playerCount);
}

public class PackGenerator : IPackGenerator
{
    private readonly Random _random = new();

    public List<Pack> GeneratePacks(Set set, int playerCount)
    {
        var foilSlotIndex = 12;
        var buckets = SetCardBuckets.Build(set);
        var packs = new List<Pack>();

        Console.WriteLine($"Common count: {buckets.Common.Count}");
        Console.WriteLine($"Uncommon count: {buckets.Uncommon.Count}");
        Console.WriteLine($"Rare count: {buckets.Rare.Count}");
        Console.WriteLine($"Mythic count: {buckets.Mythic.Count}");
        Console.WriteLine($"FableUncommon count: {buckets.FableUncommon.Count}");
        Console.WriteLine($"FableRare count: {buckets.FableRare.Count}");
        Console.WriteLine($"FableMythic count: {buckets.FableMythic.Count}");
        Console.WriteLine($"BorderlessRare count: {buckets.BorderlessRare.Count}");
        Console.WriteLine($"BorderlessMythic count: {buckets.BorderlessMythic.Count}");
        Console.WriteLine($"ReversableShock count: {buckets.ReversableShock.Count}");
        Console.WriteLine($"SpecialGuest count: {buckets.SpecialGuests.Count}");



        for (int seat = 0 ; seat < playerCount ; seat++)
        {
            for (int packNum = 1 ; packNum <= 3 ; packNum++)
            {
                var cards = GenerateSinglePack(buckets);

                packs.Add(new Pack
                {
                    PackNumber = packNum,
                    OriginalSeat = seat + 1,
                    CurrentSeat = seat + 1,
                    Cards = cards.Select((card, index) => new PackCard
                    {
                        CardId = card.Id,
                        IsPicked = false,
                        FoilType = index == foilSlotIndex 
                            ? Enums.FoilType.TraditionalFoil
                            : card.FoilType
                    }).ToList()
                });
            }
        }

        return packs;
    }

    private List<Card> GenerateSinglePack(SetCardBuckets bucket)
    {
        var pack = new List<Card>();

        // 6 commons
        for (int i = 0 ; i < 6 ; i++)
        {
            pack.Add(RandomFrom(bucket.Common));
        }

        // 1 common or special guest
        bool specialGuest = _random.NextDouble() < 0.018;
        pack.Add(specialGuest && bucket.SpecialGuests.Any()
            ? RandomFrom(bucket.SpecialGuests)
            : RandomFrom(bucket.Common));

        // 3 uncommons
        for (int i = 0 ; i < 3 ; i++)
        {
            bool fable = _random.NextDouble() < 0.10;
            pack.Add(fable && bucket.FableUncommon.Any()
                ? RandomFrom(bucket.FableUncommon)
                : RandomFrom(bucket.Uncommon));
        }

        // wildcard
        pack.Add(GenerateWildcard(bucket));

        // rare or mythic
        pack.Add(GenerateRareOrMythic(bucket));

        // foil wildcard
        pack.Add(GenerateFoilWildcard(bucket));

        // basic land
        pack.Add(GenerateBasicLand(bucket));

        return pack;
    }

    private Card GenerateWildcard(SetCardBuckets bucket)
    {
        double roll = _random.NextDouble() * 100;

        if (bucket.Common.Count == 0) { Console.WriteLine("common bucket empty"); }
        if (bucket.Uncommon.Count == 0) { Console.WriteLine("Uncommon bucket empty"); }
        if (bucket.Rare.Count == 0) { Console.WriteLine("Rare bucket empty"); }
        if (bucket.Mythic.Count == 0) { Console.WriteLine("Mythic bucket empty"); }
        if (bucket.FableUncommon.Count == 0) { Console.WriteLine("FableUncommon bucket empty"); }
        if (bucket.FableRare.Count == 0) { Console.WriteLine("FableRare bucket empty"); }
        if (bucket.FableMythic.Count == 0) { Console.WriteLine("FableMythic bucket empty"); }
        if (bucket.BorderlessRare.Count == 0) { Console.WriteLine("BorderlessRare bucket empty"); }
        if (bucket.BorderlessMythic.Count == 0) { Console.WriteLine("BorderlessMythic bucket empty"); }
        if (bucket.ReversableShock.Count == 0) { Console.WriteLine("ReversableShock bucket empty"); }
        if (bucket.SpecialGuests.Count == 0) { Console.WriteLine("SpecialGuest bucket empty"); }



        return roll switch
        {
            < 18 => RandomFrom(bucket.Common),
            < 76 => RandomFrom(bucket.Uncommon),
            < 94.5 => RandomFrom(bucket.Rare),
            < 96 => RandomFrom(bucket.Mythic),
            < 98 => RandomFrom(bucket.FableUncommon),
            < 98.8 => RandomFrom(bucket.FableRare),
            < 99 => RandomFrom(bucket.FableMythic),
            < 99.8 => RandomFrom(bucket.BorderlessRare),
            < 99.95 => RandomFrom(bucket.BorderlessMythic),
            _ => RandomFrom(bucket.ReversableShock)  
        };
    }

    private Card GenerateRareOrMythic(SetCardBuckets bucket)
    {
        double roll = _random.NextDouble() * 100;

        return roll switch
        {
            < 78.2 => RandomFrom(bucket.Rare),
            < 91.8 => RandomFrom(bucket.Mythic),
            < 96.4 => RandomFrom(bucket.FableRare),
            < 97.6 => RandomFrom(bucket.FableMythic),
            < 98.4 => RandomFrom(bucket.BorderlessRare),
            < 99.0 => RandomFrom(bucket.BorderlessMythic),
            _ => RandomFrom(bucket.ReversableShock),
        };
    }

    private Card GenerateFoilWildcard(SetCardBuckets bucket)
    {
        double roll = _random.NextDouble() * 100;

        return roll switch
        {
            < 60.4 => RandomFrom(bucket.Common),
            < 90.2 => RandomFrom(bucket.Uncommon),
            < 96.7 => RandomFrom(bucket.Rare),
            < 97.8 => RandomFrom(bucket.Mythic),
            < 98.8 => RandomFrom(bucket.FableUncommon),
            < 99.8 => RandomFrom(bucket.FableRare),
            < 99.9 => RandomFrom(bucket.FableMythic),
            < 99.95 => RandomFrom(bucket.BorderlessRare),
            < 99.99 => RandomFrom(bucket.BorderlessMythic),
            _ => RandomFrom(bucket.ReversableShock),
        };
    }

    private Card GenerateBasicLand(SetCardBuckets bucket)
    {
        double roll = _random.NextDouble() * 100;

        return roll < 0.5
            ? RandomFrom(bucket.BasicLands)
            : RandomFrom(bucket.FullArtBasicLands);
    }
    
    private Card RandomFrom(List<Card> list)
    {
        if (list == null || list.Count == 0) throw new InvalidOperationException("cant pick card from empty list");
        return list[_random.Next(list.Count)];
    }
}