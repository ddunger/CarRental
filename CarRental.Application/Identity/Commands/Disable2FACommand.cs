using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Disable2FACommand(string TotpCode) : IRequest<Result<StringResponse>>;

    public class Disable2FACommandHandler(
        UserManager<UserEntity> userManager,
        ITotp2FAService totpService,
        IUserContext userContext)
        : IRequestHandler<Disable2FACommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(Disable2FACommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return Result<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return Result<StringResponse>.Failure("User not found.", ResultError.NotFound);

            if (!user.TwoFactorEnabled)
                return Result<StringResponse>.Failure("2FA is not enabled for this account.", ResultError.Validation);

            if (string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
                return Result<StringResponse>.Failure("2FA secret key is missing or invalid.", ResultError.Internal);

            if (!totpService.ValidateCode(user.TwoFactorSecretKey, command.TotpCode))
                return Result<StringResponse>.Failure("Invalid authenticator code. Please try again.", ResultError.Validation);

            user.TwoFactorSecretKey = string.Empty;
            user.TwoFactorEnabled = false;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<StringResponse>.Failure("Failed to disable 2FA.", ResultError.Internal);

            return Result<StringResponse>.Success(new StringResponse("2FA disabled successfully."));
        }
    }
}
