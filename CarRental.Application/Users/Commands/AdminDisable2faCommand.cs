using CarRental.Application.Common.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Users.Commands
{
    public record AdminDisable2faCommand(string UserId) : IRequest<ApplicationResult<StringResponse>>;

    public class AdminDisable2faCommandHandler(
    UserManager<UserEntity> userManager
    ) : IRequestHandler<AdminDisable2faCommand, ApplicationResult<StringResponse>>

    {
        public async Task<ApplicationResult<StringResponse>> Handle
            (AdminDisable2faCommand command, CancellationToken cancellationToken)
        {

            var user = await userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);

            if (!user.TwoFactorEnabled)
                return ApplicationResult<StringResponse>.Failure("2FA is not enabled for this account.", ResultError.Validation);

            user.TwoFactorSecretKey = string.Empty;
            user.TwoFactorEnabled = false;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<StringResponse>.Failure("Failed to revoke 2FA.", ResultError.Internal);

            return ApplicationResult<StringResponse>.Success(new StringResponse("2FA revoked successfully."));
        }
    
    }
}


