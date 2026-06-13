using CarRental.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CarRental.Application.Common.User
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null) return null;

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = user.FindFirstValue(ClaimTypes.Email);
            if (id is null || email is null) return null;

            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return new CurrentUser(id, email, roles);
        }
    }
}

