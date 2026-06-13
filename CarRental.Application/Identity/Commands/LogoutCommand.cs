using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record LogoutCommand(ClientType ClientType) : IRequest<ApplicationResult<StringResponse>>;

    public class LogoutCommandHandler(
        ILogger<LogoutCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserContext userContext,
        UserManager<UserEntity> userManager)
        : IRequestHandler<LogoutCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return ApplicationResult<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return ApplicationResult<StringResponse>.Failure("User not found.", ResultError.NotFound);

            await jwtTokenGenerator.RevokeExistingTokensAsync(user, command.ClientType);

            logger.LogInformation("User {UserId} logged out successfully.", currentUser.Id);

            return ApplicationResult<StringResponse>.Success(new StringResponse("Logged out successfully."));
        }
    }
}