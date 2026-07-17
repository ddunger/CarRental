using System.Security.Claims;

namespace CarRental.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user) =>
            user.FindFirst("sub")?.Value;

        public static string? GetEmail(this ClaimsPrincipal user) =>
            user.FindFirst("email")?.Value;

        public static string? GetRole(this ClaimsPrincipal user) =>
            user.FindFirst("role")?.Value;
    }
}