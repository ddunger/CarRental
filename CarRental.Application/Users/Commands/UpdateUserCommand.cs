using CarRental.Application.Common.Responses;
using CarRental.Application.Users.Requests;
using CarRental.Application.Users.Responses;
using CarRental.Domain.Constants;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Users.Commands
{
    public record UpdateUserCommand(string UserId, UpdateUserRequest Request) : IRequest<ApplicationResult<UserResponse>>;

    public class UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IUserContext userContext,
        UserManager<UserEntity> userManager)
        : IRequestHandler<UpdateUserCommand, ApplicationResult<UserResponse>>
    {
        public async Task<ApplicationResult<UserResponse>> Handle(
            UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<UserResponse>.Failure("Unauthorized.", ResultError.Unauthorized);

            if (currentUser.Id != command.UserId && !currentUser.IsInRole(Roles.Admin))
                return ApplicationResult<UserResponse>.Failure("You can only update your own profile.", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return ApplicationResult<UserResponse>.Failure("User not found.", ResultError.NotFound);

            user.FirstName = command.Request.FirstName;
            user.LastName = command.Request.LastName;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApplicationResult<UserResponse>.Failure("Failed to update user.", ResultError.Internal);

            var roles = await userManager.GetRolesAsync(user);
            logger.LogInformation("User {UserId} profile updated.", command.UserId);
            return ApplicationResult<UserResponse>.Success(new UserResponse(
                user.Id,
                user.Email!,
                roles.FirstOrDefault(),
                user.FirstName,
                user.LastName,
                user.IsActive,
                user.TwoFactorEnabled,
                user.CreatedAt,
                user.UpdatedAt));
        }
    }
}