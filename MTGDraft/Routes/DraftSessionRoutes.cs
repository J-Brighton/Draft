using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;
using MTGDraft.Models;

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

    }
}