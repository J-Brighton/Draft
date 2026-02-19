using MTGDraft.Models;

public static class DraftSessionFactory
{
    public static DraftSession Create(Set set, int playerCount)
    {
        Console.WriteLine($"Creating session - Set: {set.Code}, PlayerCount: {playerCount}, Cards in set: {set.Cards.Count}");
        
        var session = new DraftSession
        {
            SetCode = set.Code,
            CreatedAt = DateTime.Now,
            Packs = GeneratePacks(set, playerCount)
        };

        return session;
    }

    private static List<Pack> GeneratePacks(Set set, int playerCount)
    {
        var packs = new List<Pack>();

        for (int seat = 0 ; seat < playerCount ; seat++)
        {
            for (int packNum = 1 ; packNum <= 3 ; packNum++)
            {
                packs.Add(new Pack
                {
                    PackNumber = packNum,
                    OriginalSeat = seat,
                    Cards = set.Cards.OrderBy(_ => Guid.NewGuid())
                    // this currently selects 15 cards from 1-15 in the ECL json. dogshit
                            .Take(15)
                            .Select(card => new PackCard {CardId = card.Id})
                            .ToList()
                });
            }
        }
        Console.WriteLine($"PACKS: {packs.Count}");
        foreach (var pack in packs)
        {
            Console.WriteLine($"  Seat={pack.OriginalSeat}, PackNum={pack.PackNumber}, Cards={pack.Cards.Count}");
        }
        return packs;
    }
}