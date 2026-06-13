using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Disable2FAWithRecoveryCodeCommand(string Email, string RecoveryCode)
        : IRequest<Result<StringResponse>>;

    public class Disable2FAWithRecoveryCodeCommandHandler(
        UserManager<UserEntity> userManager)
        : IRequestHandler<Disable2FAWithRecoveryCodeCommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(
            Disable2FAWithRecoveryCodeCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return Result<StringResponse>.Failure("User not found.", ResultError.NotFound);

            var redeemResult = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, command.RecoveryCode);
            if (!redeemResult.Succeeded)
                return Result<StringResponse>.Failure("Invalid or already-used recovery code.", ResultError.Unauthorized);

            user.TwoFactorEnabled = false;
            user.TwoFactorSecretKey = null;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<StringResponse>.Failure("Failed to disable 2FA.", ResultError.Internal);

            return Result<StringResponse>.Success(
                new StringResponse("2FA disabled successfully using recovery code."));
        }
    }
}
