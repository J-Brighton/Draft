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

        // register
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

        // login
        group.MapPost("/login", async (LoginRequest req, DraftContext context, JwtTokenService jwt, HttpContext http) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null) return Results.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();

            var token = jwt.GenerateToken(user);

            http.Response.Cookies.Append("auth", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(12)
            });

            return Results.Ok(new { message = "logged in" });
        });
    }
}