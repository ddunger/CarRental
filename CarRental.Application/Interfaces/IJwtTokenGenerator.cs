using CarRental.Application.Common.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;

namespace CarRental.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<TokenResult> GenerateTokenAsync(UserEntity user, ClientType clientType = ClientType.Web);
        Task<TokenResult?> RefreshAccessTokenAsync(string refreshToken);
        Task RevokeExistingTokensAsync(UserEntity user, ClientType clientType);

    }
}
