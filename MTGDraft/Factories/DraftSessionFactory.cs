using MTGDraft.Models;

public static class DraftSessionFactory
{
    public static DraftSession Create(Set set, int playerCount)
    {
        Console.WriteLine($"Creating session - Set: {set.Code}, PlayerCount: {playerCount}, Cards in set: {set.Cards.Count}");
        
        var session = new DraftSession
        {
            SetCode = set.Code,
            PlayerCount = playerCount,
            DraftState = "NotStarted",
            CreatedAt = DateTime.Now,
            Packs = [] // make packs empty initially, instead generate when draft starts
        };

        return session;
    }

    public static List<Pack> GeneratePacks(Set set, int playerCount)
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
        // https://magic.wizards.com/en/news/feature/collecting-lorwyn-eclipsed
        // this section is still not fully correct.
        // does not use special guests or alternate arts, and the wildcard distribution is not right.
        // random selection yields 32, 36, 24, 8 for C, U, R, M respectively instead of 18, 58, 19, 2 
        
        var random = new Random();
        var packCards = new List<Card>();
        var maxCardNumber = set.Cards.Max(c => c.CardNumber);

        // slot 1-7 common (ignore special guests & basic lands)
        var commonCards = set.Cards.Where(card => card.Rarity == "C" && card.CardNumber < maxCardNumber - 5).ToList();
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

        // one wildcard and another foil wildcard  (not weighted 18, 58, 19, 2)
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
        var landCards = set.Cards.Where(card => card.CardNumber > maxCardNumber - 5).ToList();
        var landCard = landCards[random.Next(landCards.Count)];

        packCards.Add(landCard);
        return packCards;
    }
}