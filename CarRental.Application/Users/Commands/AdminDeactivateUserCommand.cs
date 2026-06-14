using CarRental.Application.Common.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Users.Commands
{
    public record AdminDeactivateUserCommand(string UserId) : IRequest<ApplicationResult<StringResponse>>;

    public class AdminDeactivateUserCommandHandler(
        ILogger<AdminDeactivateUserCommandHandler> logger,
        UserManager<UserEntity> userManager)
        : IRequestHandler<AdminDeactivateUserCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            AdminDeactivateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);

            if (!user.IsActive)
                return ApplicationResult<StringResponse>.Failure("User account is already deactivated.", ResultError.Validation);

            user.IsActive = false;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<StringResponse>.Failure("Failed to deactivate user.", ResultError.Internal);

            logger.LogInformation("User {UserId} deactivated.", command.UserId);
            return ApplicationResult<StringResponse>.Success(new StringResponse("User account deactivated successfully."));
        }
    }
}