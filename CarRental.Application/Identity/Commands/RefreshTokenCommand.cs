using CarRental.Domain.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Application.Identity.Responses;
using CarRental.Application.Interfaces;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequest RefreshTokenDto)
     : IRequest<ApplicationResult<LoginResponse>>;

    public class RefreshTokenCommandHandler(
        ILogger<RefreshTokenCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator)
        : IRequestHandler<RefreshTokenCommand, ApplicationResult<LoginResponse>>
    {
        public async Task<ApplicationResult<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await jwtTokenGenerator.RefreshAccessTokenAsync(command.RefreshTokenDto.RefreshToken);
            if (result is null)
            {
                logger.LogWarning("Invalid or expired refresh token used.");
                return ApplicationResult<LoginResponse>.Failure("Invalid or expired refresh token.", ResultError.Unauthorized);
            }

            return ApplicationResult<LoginResponse>.Success(new LoginResponse(
                AccessToken: result.AccessToken,
                RefreshToken: result.RefreshToken,
                RoleName: result.RoleName));
        }
    }
}
