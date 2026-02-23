using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Card;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;

namespace MTGDraft.Routes;

public static class DraftSessionRoutes
{
    public static void MapDraftSessionRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/DraftSession");

        // get all the draft sessions
        group.MapGet("/", async (DraftContext context) =>
        {
            var sessions = await context.DraftSessions
                .Select(draft => new DraftSessionSummaryDTO(
                    draft.Id,
                    draft.SetCode,
                    draft.CreatedAt
                )).ToListAsync();
            
            return Results.Ok(sessions);
        });

        // get a specific draft session
        group.MapGet("/{id}", async (int id, DraftContext context) =>
        {
            var session = await context.DraftSessions
                .Where(draft => draft.Id == id)
                .Select(draft => new DraftSessionDTO(
                    draft.Id,
                    draft.SetCode,
                    draft.CreatedAt,
                    draft.Packs.Select(pack => new PackDTO(
                        pack.Id,
                        pack.PackNumber,
                        pack.OriginalSeat,
                        pack.Cards.Select(packcard => new PackCardDTO(
                            packcard.Id,
                            packcard.IsPicked,
                            packcard.IsFoil,
                            new CardDTO(
                                packcard.Card.Id,
                                packcard.Card.Name,
                                packcard.Card.Rarity,
                                packcard.Card.SetCode,  
                                packcard.Card.CardNumber,
                                packcard.Card.SetId
                            )
                        )).ToList()
                    )).ToList()
                )).FirstOrDefaultAsync();

            if (session is null) {
                return Results.NotFound();
            }

            return Results.Ok(session);
        });

        // create a draft session
        group.MapPost("/", async (AddDraftSessionDTO dto, DraftSessionService service) => {
            try {
                var session = await service.CreateDraftSession(dto.SetCode, dto.PlayerCount);

                var resultDto = new DraftSessionSummaryDTO(
                    session.Id,
                    session.SetCode,
                    session.CreatedAt
                );

                return Results.Created($"/DraftSession/{session.Id}", resultDto);
            }

            catch (ArgumentException ex) {
                return Results.BadRequest(ex.Message);
            }
        });

        // delete a draft session
        group.MapDelete("/{id}", async (int id, DraftContext context) => {
            var session = await context.DraftSessions.FindAsync(id);

            if (session is null) {
                return Results.NotFound();
            }

            context.DraftSessions.Remove(session);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}