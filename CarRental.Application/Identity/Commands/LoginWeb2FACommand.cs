using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Application.Identity.Responses;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record LoginWeb2FACommand(Login2FARequest Dto, ClientType ClientType)
      : IRequest<Result<LoginResponse>>;

    public class LoginWith2FACommandHandler(
        ILogger<LoginWith2FACommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        ITotp2FAService totpService,
        UserManager<UserEntity> userManager)
        : IRequestHandler<LoginWeb2FACommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(LoginWeb2FACommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.Dto.Email);
            if (user is null)
                return Result<LoginResponse>.Failure("Invalid credentials provided.", ResultError.Unauthorized);

            if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey))
                return Result<LoginResponse>.Failure("2FA is not enabled for this account.", ResultError.Validation);

            if (!totpService.ValidateCode(user.TwoFactorSecretKey, command.Dto.Code))
            {
                logger.LogWarning("Invalid 2FA code for user: {Email}.", command.Dto.Email);
                return Result<LoginResponse>.Failure("Invalid 2FA code.", ResultError.Unauthorized);
            }           

            await jwtTokenGenerator.RevokeExistingTokensAsync(user, command.ClientType);
            var tokens = await jwtTokenGenerator.GenerateTokenAsync(user, command.ClientType);

            logger.LogInformation("2FA login successful for user: {Email}.", command.Dto.Email);

            return Result<LoginResponse>.Success(new LoginResponse(
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                RoleName: tokens.RoleName));
        }
    }
}