using CarRental.Domain.Results;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarRental.Infrastructure.Authentication
{
    public class JwtTokenGenerator(
        IConfiguration configuration,
        AppDbContext dbContext,
        UserManager<UserEntity> userManager) : IJwtTokenGenerator
    {
        private readonly IConfigurationSection _jwtSettings = configuration.GetSection("Jwt");

        public async Task<TokenResult> GenerateTokenAsync(UserEntity user, ClientType clientType = ClientType.Web)
        {
            var roles = await userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault();
            var accessToken = GenerateAccessToken(user, roles);
            var refreshToken = await CreateAndStoreRefreshTokenAsync(user, clientType);
            return new TokenResult(accessToken, refreshToken, roleName);
        }

        public async Task<TokenResult?> RefreshAccessTokenAsync(string refreshToken)
        {
            var stored = await dbContext.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken);

            if (stored is null || stored.IsRevoked || stored.ExpiresAtUtc < DateTime.UtcNow)
                return null;

            stored.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            var roles = await userManager.GetRolesAsync(stored.User);
            var accessToken = GenerateAccessToken(stored.User, roles);
            var newRefreshToken = await CreateAndStoreRefreshTokenAsync(stored.User, stored.ClientType);
            return new TokenResult(accessToken, newRefreshToken, roles.FirstOrDefault());
        }

        public async Task RevokeExistingTokensAsync(UserEntity user, ClientType clientType)
        {
            var tokensToRevoke = await dbContext.RefreshTokens
                .AsTracking()
                .Where(rt => rt.UserId == user.Id
                    && rt.ClientType == clientType
                    && !rt.IsRevoked
                    && rt.ExpiresAtUtc > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokensToRevoke)
                token.IsRevoked = true;

            await dbContext.SaveChangesAsync();
        }

        // --- Private helpers ---

      

        private async Task<string> CreateAndStoreRefreshTokenAsync(UserEntity user, ClientType clientType)
        {
            var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiryDays = clientType == ClientType.Mobile
                ? Convert.ToDouble(_jwtSettings["MobileRefreshTokenExpiryInDays"] ?? "30")
                : Convert.ToDouble(_jwtSettings["RefreshTokenExpiryInDays"] ?? "7");

            var refreshToken = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                Token = tokenValue,
                UserId = user.Id,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expiryDays),
                IsRevoked = false,
                ClientType = clientType
            };

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();
            return tokenValue;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["Key"]!);
            return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        }

        private static List<Claim> GetClaims(UserEntity user, IList<string> roles)
        {
           var claims = new List<Claim>
           {
               new(JwtRegisteredClaimNames.Sub, user.Id),
               new(JwtRegisteredClaimNames.Email, user.Email!),
               new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
               new(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
           };

            var role = roles.FirstOrDefault();
            if (role is not null)
                claims.Add(new Claim("role", role));

            return claims;
        }

        private string GenerateAccessToken(UserEntity user, IList<string> roles)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings["Issuer"],
                Audience = _jwtSettings["Audience"],
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings["ExpiryInMinutes"])),
                SigningCredentials = GetSigningCredentials(),
                Subject = new ClaimsIdentity(GetClaims(user, roles))
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }

    }
}