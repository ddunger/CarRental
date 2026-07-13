using CarRental.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace CarRental.Application.Common.User
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null) return null;

            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var email = user.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (id is null || email is null) return null;

            var roles = user.FindAll("role").Select(c => c.Value);

            return new CurrentUser(id, email, roles);
        }
    }
}

