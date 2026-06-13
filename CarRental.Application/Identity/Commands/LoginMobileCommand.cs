using CarRental.Domain.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Application.Identity.Responses;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record LoginMobileCommand(LoginUserRequest LoginUserDto, ClientType ClientType)
     : IRequest<ApplicationResult<LoginResponse>>;

    public class LoginMobileCommandHandler(
        ILogger<LoginMobileCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        UserManager<UserEntity> userManager)
        : IRequestHandler<LoginMobileCommand, ApplicationResult<LoginResponse>>
    {
        public async Task<ApplicationResult<LoginResponse>> Handle(LoginMobileCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.LoginUserDto.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, command.LoginUserDto.Password))
            {
                logger.LogWarning("Failed mobile login attempt for email: {Email}.", command.LoginUserDto.Email);
                return ApplicationResult<LoginResponse>.Failure("Invalid credentials provided.", ResultError.Unauthorized);
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Inactive user attempted mobile login: {Email}.", command.LoginUserDto.Email);
                return ApplicationResult<LoginResponse>.Failure("Your account has been deactivated.", ResultError.Unauthorized);
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                logger.LogWarning("Unconfirmed email mobile login attempt: {Email}.", command.LoginUserDto.Email);
                return ApplicationResult<LoginResponse>.Failure("Please confirm your email before logging in.", ResultError.Unauthorized);
            }

           

            await jwtTokenGenerator.RevokeExistingTokensAsync(user, command.ClientType);
            var tokens = await jwtTokenGenerator.GenerateTokenAsync(user, command.ClientType);

            logger.LogInformation("Mobile login successful for user: {Email}.", command.LoginUserDto.Email);

            return ApplicationResult<LoginResponse>.Success(new LoginResponse(
                    AccessToken: tokens.AccessToken,
                    RefreshToken: tokens.RefreshToken,
                    RoleName: tokens.RoleName));
        }
    }
}
