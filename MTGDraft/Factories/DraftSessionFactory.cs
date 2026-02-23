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
                var cards = GeneratePack(set);
                packs.Add(new Pack
                {
                    PackNumber = packNum,
                    OriginalSeat = seat,
                    Cards = cards.Select((card, index) => new PackCard {
                        CardId = card.Id,
                        IsPicked = false,
                        IsFoil = index == 10  // foil wildcard is at index 10
                    }).ToList()
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

    private static List<Card> GeneratePack(Set set)
    {
        // slot 1-6 common
        // slot 7 common or special guest card
        // slot 8-10 uncommon
        // slot 11 non-foil wildcard
        // slot 14 foil wildcard
        // slot 12 rare or mythic
        // slot 13 basic land

        var random = new Random();
        var packCards = new List<Card>();

        // slot 1-7 common (ignore special guests)
        var commonCards = set.Cards.Where(card => card.Rarity == "C").ToList();
        for (int i = 0 ; i < 7 ; i++)
        {
            var commonCard = commonCards[random.Next(commonCards.Count)];
            packCards.Add(commonCard);
        }

        // slot 8-10 uncommon
        var uncommonCards = set.Cards.Where(card => card.Rarity == "U").ToList();
        for (int i = 0 ; i < 3 ; i++)
        {
            var uncommonCard = uncommonCards[random.Next(uncommonCards.Count)];
            packCards.Add(uncommonCard);
        }

        // one wildcard and another foil wildcard 
        var wildCards = set.Cards.ToList();
        for (int i = 0 ; i < 2 ; i++) {
            var wildCard = wildCards[random.Next(wildCards.Count)];
            packCards.Add(wildCard);
        }

        // add a rare or mythic
        var rareOrMythicCards = set.Cards.Where(card => card.Rarity == "R" || card.Rarity == "M").ToList();
        var mythicCards = set.Cards.Where(card => card.Rarity == "M").ToList();

        var isMythic = random.NextDouble() < 0.125; // 1 in 8 packs have a mythic instead of a rare
        var rareOrMythicCard = isMythic && mythicCards.Count > 0
            ? mythicCards[random.Next(mythicCards.Count)]
            : rareOrMythicCards[random.Next(rareOrMythicCards.Count)];
        packCards.Add(rareOrMythicCard);

        // Basic lands are the last 5 cards in the set by CardNumber
        var maxCardNumber = set.Cards.Max(c => c.CardNumber);
        var landCards = set.Cards.Where(card => card.CardNumber > maxCardNumber - 5).ToList();
        var landCard = landCards[random.Next(landCards.Count)];

        packCards.Add(landCard);
        return packCards;
    }
}