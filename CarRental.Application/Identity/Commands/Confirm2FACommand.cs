using CarRental.Domain.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Confirm2FACommand(string Code) : IRequest<ApplicationResult<IEnumerable<string>>>;

    public class Confirm2FACommandHandler(
        UserManager<UserEntity> userManager,
        ITotp2FAService totpService,
        IUserContext userContext)
        : IRequestHandler<Confirm2FACommand, ApplicationResult<IEnumerable<string>>>
    {
        public async Task<ApplicationResult<IEnumerable<string>>> Handle(
            Confirm2FACommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<IEnumerable<string>>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return ApplicationResult<IEnumerable<string>>.Failure("User not found.", ResultError.NotFound);

            if (string.IsNullOrEmpty(user.TwoFactorSecretKey))
                return ApplicationResult<IEnumerable<string>>.Failure(
                    "2FA setup has not been initiated. Please call /2fa/enable first.", ResultError.Validation);

            if (!totpService.ValidateCode(user.TwoFactorSecretKey, command.Code))
                return ApplicationResult<IEnumerable<string>>.Failure("Invalid 2FA code. Please try again.", ResultError.Validation);

            user.TwoFactorEnabled = true;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<IEnumerable<string>>.Failure("Failed to enable 2FA.", ResultError.Internal);

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return ApplicationResult<IEnumerable<string>>.Success(recoveryCodes!);
        }
    }
}