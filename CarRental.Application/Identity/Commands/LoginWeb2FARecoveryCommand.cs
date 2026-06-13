using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Application.Identity.Responses;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record LoginWeb2FARecoveryCommand(Login2FARecoveryRequest Dto, ClientType ClientType)
       : IRequest<Result<LoginResponse>>;

    public class LoginWeb2FARecoveryCommandHandler(
        ILogger<LoginWeb2FARecoveryCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        UserManager<UserEntity> userManager)
        : IRequestHandler<LoginWeb2FARecoveryCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(LoginWeb2FARecoveryCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.Dto.Email);
            if (user is null)
                return Result<LoginResponse>.Failure("Invalid credentials provided.", ResultError.Unauthorized);

            if (!user.TwoFactorEnabled)
                return Result<LoginResponse>.Failure("2FA is not enabled for this account.", ResultError.Validation);

            var redeemResult = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, command.Dto.RecoveryCode);
            if (!redeemResult.Succeeded)
            {
                logger.LogWarning("Invalid or used recovery code for user: {Email}.", command.Dto.Email);
                return Result<LoginResponse>.Failure("Invalid or already-used recovery code.", ResultError.Unauthorized);
            } 

            await jwtTokenGenerator.RevokeExistingTokensAsync(user, command.ClientType);
            var tokens = await jwtTokenGenerator.GenerateTokenAsync(user, command.ClientType);

            logger.LogInformation("Recovery code login successful for user: {Email}.", command.Dto.Email);

            return Result<LoginResponse>.Success(new LoginResponse(
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                RoleName: tokens.RoleName));
        }
    }
}