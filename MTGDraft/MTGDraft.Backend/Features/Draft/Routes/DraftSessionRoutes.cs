using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Features.Draft.DTOs;
using MTGDraft.Features.Players.DTOs;

namespace MTGDraft.Features.Draft.Routes;

public static class DraftSessionRoutes
{
    public static void MapDraftSessionRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/DraftSession");

        group.RequireAuthorization();

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

                return Results.Created($"api/DraftSession/{session.Id}", resultDto);
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
        group.MapPost("/{id}/Join", async (int id, DraftSessionService service, HttpContext http) =>
        {
            if (!AuthHelper.GetUserId(http, out var userId)) return Results.Unauthorized();

            await service.JoinDraftSession(id, userId);
            return Results.Ok($"{userId} has joined draft session {id}");
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