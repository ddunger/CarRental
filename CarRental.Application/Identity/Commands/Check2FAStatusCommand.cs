using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Application;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRental.Application.Identity.Commands
{
    public record Check2FAStatusCommand() : IRequest<Result<TwoFactorStatusResponse>>;

    public class Check2FAStatusCommandHandler(
        UserManager<UserEntity> userManager,
        IUserContext userContext)
        : IRequestHandler<Check2FAStatusCommand, Result<TwoFactorStatusResponse>>
    {
        public async Task<Result<TwoFactorStatusResponse>> Handle(
            Check2FAStatusCommand command, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser is null)
                return Result<TwoFactorStatusResponse>.Failure("Unauthorized", ResultError.Unauthorized);

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user is null)
                return Result<TwoFactorStatusResponse>.Failure("User not found.", ResultError.NotFound);

            return Result<TwoFactorStatusResponse>.Success(user.TwoFactorEnabled
                ? new TwoFactorStatusResponse(true, "2FA is enabled for this account.")
                : new TwoFactorStatusResponse(false, "2FA is not enabled for this account."));
        }
    }
}
