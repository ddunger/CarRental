using CarRental.Application.Common.Results;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        private string GenerateAccessToken(UserEntity user, IList<string> roles)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user, roles);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

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
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtSettings["Issuer"],
                audience: _jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings["ExpiryInMinutes"])),
                signingCredentials: signingCredentials
            );
        }
    }
}