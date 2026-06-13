using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record ChangePasswordCommand(ChangePasswordRequest ChangePassword) : IRequest<ApplicationResult<StringResponse>>;

    public class ChangePasswordCommandHandler(
        ILogger<ChangePasswordCommandHandler> logger,
        IUserContext userContext,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ChangePasswordCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);      

            var user = await userManager.FindByEmailAsync(currentUser.Email);
            if (user is null)
            {
                logger.LogError("Authenticated user with email {Email} not found in the database.", currentUser.Email);
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);
            }

            var result = await userManager.ChangePasswordAsync(user, command.ChangePassword.OldPassword, command.ChangePassword.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Password change failed for {Email}: {Errors}.", currentUser.Email, errors);
                return ApplicationResult<StringResponse>.Failure(errors, ResultError.Validation);
            }

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("Password changed successfully."));
        }
    }
}
