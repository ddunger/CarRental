using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Disable2FAWithRecoveryCodeCommand(string Email, string RecoveryCode)
        : IRequest<ApplicationResult<StringResponse>>;

    public class Disable2FAWithRecoveryCodeCommandHandler(
        UserManager<UserEntity> userManager)
        : IRequestHandler<Disable2FAWithRecoveryCodeCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            Disable2FAWithRecoveryCodeCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);

            var redeemResult = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, command.RecoveryCode);
            if (!redeemResult.Succeeded)
                return ApplicationResult<StringResponse>.Failure("Invalid or already-used recovery code.", ResultError.Unauthorized);

            user.TwoFactorEnabled = false;
            user.TwoFactorSecretKey = null;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<StringResponse>.Failure("Failed to disable 2FA.", ResultError.Internal);

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("2FA disabled successfully using recovery code."));
        }
    }
}
