using System.Security.Claims;

public static class AuthHelper
{
    public static bool GetUserId(HttpContext http, out int userId)
    {
        var idClaim = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out userId);
    }
}