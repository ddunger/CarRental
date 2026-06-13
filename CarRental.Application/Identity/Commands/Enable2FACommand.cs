using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Enable2FACommand() : IRequest<ApplicationResult<StringResponse>>;

    public class Enable2FACommandHandler(
        UserManager<UserEntity> userManager,
        ITotp2FAService totpService,
        IUserContext userContext,
        IQRCodeService qrService)
        : IRequestHandler<Enable2FACommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(Enable2FACommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);

            if (user.TwoFactorEnabled)
                return ApplicationResult<StringResponse>.Failure("2FA is already enabled for this account.", ResultError.Conflict);

            var secret = totpService.GenerateSecretKey();
            user.TwoFactorSecretKey = secret;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<StringResponse>.Failure("Failed to save 2FA secret.", ResultError.Internal);

            var qrCodeBytes = qrService.GenerateQrCodeUri(user.Email!, secret);
            var base64QrCode = Convert.ToBase64String(qrCodeBytes);

            return ApplicationResult<StringResponse>.Success(new StringResponse(base64QrCode));
        }
    }
}

