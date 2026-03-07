using System.Text.Json;
using System.Text.Json.Serialization;
using MTGDraft.Models;

namespace MTGDraft.Data;

public static class SeedData
{
    public static async Task SeedAsync(DraftContext context)
    {
        var setFiles = Directory.GetFiles("Data/Sets", "*.json");

        if (setFiles.Length == 0)
        {
            Console.WriteLine("No set JSON files");
            return;
        }

        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        foreach (var file in setFiles)
        {
            var json = await File.ReadAllTextAsync(file);
            var import = JsonSerializer.Deserialize<SetImportDTO>(json, options);

            if (import is null)
            {
                Console.WriteLine($"faild to deserialise {file}");
                continue;
            }

            if (context.Sets.Any(s => s.Code == import.Code))
            {
                Console.WriteLine($"set {import.Code} already imported, skipping");
                continue;
            }

            var set = new Set
            {
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
                SetId = set.Id,
                Treatment = card.Treatment,
                FoilType = card.FoilType,
                IsBasicLand = card.IsBasicLand
            }).ToList();

            context.Cards.AddRange(cards);
            await context.SaveChangesAsync();
        }
    }
}