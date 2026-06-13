using CarRental.Application.Common.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Confirm2FACommand(string Code) : IRequest<Result<IEnumerable<string>>>;

    public class Confirm2FACommandHandler(
        UserManager<UserEntity> userManager,
        ITotp2FAService totpService,
        IUserContext userContext)
        : IRequestHandler<Confirm2FACommand, Result<IEnumerable<string>>>
    {
        public async Task<Result<IEnumerable<string>>> Handle(
            Confirm2FACommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return Result<IEnumerable<string>>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return Result<IEnumerable<string>>.Failure("User not found.", ResultError.NotFound);

            if (string.IsNullOrEmpty(user.TwoFactorSecretKey))
                return Result<IEnumerable<string>>.Failure(
                    "2FA setup has not been initiated. Please call /2fa/enable first.", ResultError.Validation);

            if (!totpService.ValidateCode(user.TwoFactorSecretKey, command.Code))
                return Result<IEnumerable<string>>.Failure("Invalid 2FA code. Please try again.", ResultError.Validation);

            user.TwoFactorEnabled = true;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<IEnumerable<string>>.Failure("Failed to enable 2FA.", ResultError.Internal);

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return Result<IEnumerable<string>>.Success(recoveryCodes!);
        }
    }
}