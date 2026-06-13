using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Application.Identity.Responses;
using CarRental.Application.Interfaces;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequest RefreshTokenDto)
     : IRequest<Result<LoginResponse>>;

    public class RefreshTokenCommandHandler(
        ILogger<RefreshTokenCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator)
        : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await jwtTokenGenerator.RefreshAccessTokenAsync(command.RefreshTokenDto.RefreshToken);
            if (result is null)
            {
                logger.LogWarning("Invalid or expired refresh token used.");
                return Result<LoginResponse>.Failure("Invalid or expired refresh token.", ResultError.Unauthorized);
            }

            return Result<LoginResponse>.Success(new LoginResponse(
                AccessToken: result.AccessToken,
                RefreshToken: result.RefreshToken,
                RoleName: result.RoleName));
        }
    }
}
