using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record LogoutCommand(ClientType ClientType) : IRequest<Result<StringResponse>>;

    public class LogoutCommandHandler(
        ILogger<LogoutCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserContext userContext,
        UserManager<UserEntity> userManager)
        : IRequestHandler<LogoutCommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return Result<StringResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return Result<StringResponse>.Failure("User not found.", ResultError.NotFound);

            await jwtTokenGenerator.RevokeExistingTokensAsync(user, command.ClientType);

            logger.LogInformation("User {UserId} logged out successfully.", currentUser.Id);

            return Result<StringResponse>.Success(new StringResponse("Logged out successfully."));
        }
    }
}