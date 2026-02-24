using System.Text.RegularExpressions;
using MTGDraft.Data;
using MTGDraft.DTOs.PackCard;

namespace MTGDraft.Routes;

public static class PackCardRoutes
{
    const string GetPackCardRoute = "GetPackCard";


    public static void MapPackCardRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/packcards");

        group.MapPost("/{packId}/{id}", async (int packId, int id, UpdatePackCardDTO updateDTO, DraftContext context) =>
        {
            var packCard = await context.PackCards.FindAsync(id);

            if (packCard is null) {
                return Results.NotFound();
            }

            packCard.IsPicked = updateDTO.IsPicked;

            await context.SaveChangesAsync();

            return Results.Ok();
        }).WithName(GetPackCardRoute);
    }
}
