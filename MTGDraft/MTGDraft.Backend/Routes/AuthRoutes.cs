using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.DTOs.Auth;

namespace MTGDraft.Routes;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        // create/register user
        group.MapPost("/register", async (RegisterRequest req, DraftContext context) =>
        {
            if (await context.Users.AnyAsync(u => u.Email == req.Email))
                return Results.BadRequest("Email already used");

            var user = new User
            {
                Email = req.Email,
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Results.Ok("registered");
        });

        // get user details
        group.MapGet("/{userId}", async (int userId, DraftContext context) =>
        {
            
        });

        // update user details

        // delete user

        // login
        group.MapPost("/login", async (LoginRequest req, DraftContext context, JwtTokenService jwt, HttpContext http) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null) return Results.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();

            var token = jwt.GenerateToken(user);

            return Results.Ok(new { token });
        });    
    
        // logout

    
    }
}