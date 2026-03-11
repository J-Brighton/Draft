using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.DTOs.User;
using MTGDraft.DTOs.Auth;

namespace MTGDraft.Routes;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/auth");

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
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ArgumentException("invalid userId");

            var userDto = new UserDTO(
                Id: user.Id,
                Email: user.Email,
                Username: user.Username,
                Role: user.Role
            );

            return Results.Ok(userDto);
        });

        // update user details
        group.MapPatch("/{userId}", async (int userId, UpdateUserDTO updateDTO, DraftContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Results.NotFound("user not found");

            if (!string.IsNullOrWhiteSpace(updateDTO.Email)) user.Email = updateDTO.Email;
            if (!string.IsNullOrWhiteSpace(updateDTO.Username)) user.Username = updateDTO.Username;
            if (!string.IsNullOrWhiteSpace(updateDTO.Role)) user.Role = updateDTO.Role;

            await context.SaveChangesAsync();

            var userDTO = new UserDTO(
                Id: user.Id,
                Email: user.Email,
                Username: user.Username,
                Role: user.Role
            );

            return Results.Ok(userDTO);
        });

        // delete user
        group.MapDelete("/{userId}", async (int userId, DraftContext context) =>
        {
            var user = await context.Users
                .Include(u => u.Players)
                .FirstOrDefaultAsync(u => u.Id == userId);    
            if (user == null) return Results.NotFound("user not found");

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return Results.Ok($"User {userId} deleted");
        });

        // login
        group.MapPost("/login", async (LoginRequest req, DraftContext context, JwtTokenService jwt, HttpContext http) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null) return Results.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();

            var token = jwt.GenerateToken(user);

            return Results.Ok(new { token });
        });    
    }
}