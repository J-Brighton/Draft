using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Card;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;
using MTGDraft.DTOs.Player;

namespace MTGDraft.Routes;

public static class DraftSessionRoutes
{
    public static void MapDraftSessionRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/DraftSession");

        // create a draft session
        group.MapPost("/", async (AddDraftSessionDTO dto, DraftSessionService service) => {
            try {
                var session = await service.CreateDraftSession(dto.SetCode, dto.PlayerCount);

                var resultDto = new DraftSessionSummaryDTO(
                    session.Id,
                    session.SetCode,
                    session.PlayerCount,
                    session.CurrentPickIndex,
                    session.CurrentPackNumber,
                    session.DraftPlayers.Select(p => new PlayerSessionSummaryDTO(
                        p.Id,
                        p.Name,
                        p.IsBot,
                        p.DraftSessionId,
                        p.DraftSessionSeat,
                        p.HasPickedThisRound
                    )).ToList(),
                    session.DraftState,
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

            // ATTEMPTING TO DECOUPLE PLAYERS FROM DRAFT SESSION BEFORE DELETING
            var playersInSession = await context.Players.Where(p => p.DraftSessionId == id).ToListAsync();

            // ORPHAN THE PLAYER BEFORE DELETING DRAFT
            foreach (var player in playersInSession) {
                player.DraftSessionId = null;
            }

            context.DraftSessions.Remove(session);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    
        // join a draft session
        group.MapPost("/{id}/Join/{playerId}", async (int id, int playerId, DraftSessionService service) =>
        {
            try {
                await service.JoinDraftSession(id, playerId);
                return Results.Ok();
            }
            catch (ArgumentException ex) {
                return Results.BadRequest(ex.Message);
            }
        });

        // start a draft session
        group.MapPost("/{id}/Start", async (int id, DraftSessionService service) =>
        {
            try
            {
                var session = await service.StartDraftSession(id);
                var sessionDTO = new DraftSessionSummaryDTO(
                    Id: session.Id,
                    SetCode: session.SetCode,
                    PlayerCount: session.PlayerCount,
                    CurrentPickIndex: session.CurrentPickIndex,
                    CurrentPackNumber: session.CurrentPackNumber,
                    Players: session.DraftPlayers
                        .Select(p => new PlayerSessionSummaryDTO(
                            Id: p.Id, 
                            Name: p.Name, 
                            IsBot: p.IsBot,
                            DraftSessionId: p.DraftSessionId,
                            DraftSessionSeat: p.DraftSessionSeat,
                            HasPicked: p.HasPickedThisRound
                        )).ToList(),
                    DraftState: session.DraftState,
                    CreatedAt: session.CreatedAt
                );
                return Results.Ok(sessionDTO);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

    }
}