using System.Text.Json;
using MTGDraft.Models;

namespace MTGDraft.Data;

public static class SeedData
{
    public static async Task SeedAsync(DraftContext context)
    {
        if (context.Sets.Any(set => set.Code == "ECL"))
        {
            return;
        }

        var json = await File.ReadAllTextAsync("Data/Sets/lorwyn_eclipsed.json");

        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };

        var import = JsonSerializer.Deserialize<SetImportDTO>(json, options);

        if (import is null) {
            Console.WriteLine("Import is null - deserialization failed");
            return;
        }

        Console.WriteLine($"Loaded set: {import.Code}, Cards in import: {import.Cards.Count}");

        var set = new Set {
            Code = import.Code,
            Name = import.Name
        };

        context.Sets.Add(set);
        await context.SaveChangesAsync();

        var cards = import.Cards.Select(card => new Card {
            Name = card.Name,
            Rarity = card.Rarity,
            CardNumber = card.CardNumber,
            SetCode = set.Code,
            SetId = set.Id
        }).ToList();

        Console.WriteLine($"Adding {cards.Count} cards to context");
        context.Cards.AddRange(cards);
        await context.SaveChangesAsync();
        Console.WriteLine("Cards saved");

    }
}