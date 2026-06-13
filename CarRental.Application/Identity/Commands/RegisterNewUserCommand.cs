using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Application.Events;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace CarRental.Application.Identity.Commands
{
    public record RegisterNewUserCommand(RegisterUserRequest dto) : IRequest<ApplicationResult<StringResponse>>;


    public class RegisterNewUserCommandHandler(
        ILogger<RegisterNewUserCommandHandler> logger,
        IPublisher publisher,
        UserManager<UserEntity> userManager)
        : IRequestHandler<RegisterNewUserCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.dto.Email) || string.IsNullOrEmpty(command.dto.Password))
            {
                logger.LogError("Email or password provided are null or empty.");
                return ApplicationResult<StringResponse>.Failure("Invalid registration data provided.", ResultError.Validation);
            }

            var existingUser = await userManager.FindByEmailAsync(command.dto.Email);
            if (existingUser is not null)
            {
                logger.LogError("User with email {Email} already exists.", command.dto.Email);
                return ApplicationResult<StringResponse>.Failure($"User with email '{command.dto.Email}' already exists.", ResultError.Conflict);
            }

            var result = await userManager.CreateAsync(new UserEntity
            {
                FirstName = string.IsNullOrEmpty(command.dto.FirstName) ? null : command.dto.FirstName,
                LastName = string.IsNullOrEmpty(command.dto.LastName) ? null : command.dto.LastName,
                UserName = command.dto.Email,
                Email = command.dto.Email,
                PasswordHash = command.dto.Password
            }, command.dto.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Error creating account {Email}: {Errors}", command.dto.Email, errors);
                return ApplicationResult<StringResponse>.Failure(errors, ResultError.Validation);
            }

            var createdUser = await userManager.FindByEmailAsync(command.dto.Email);
            if (createdUser is null)
            {
                logger.LogError("User not found after creation {Email}.", command.dto.Email);
                return ApplicationResult<StringResponse>.Failure("Failed to complete registration.", ResultError.Internal);
            }

            logger.LogInformation("Sending confirmation email to {Email}.", command.dto.Email);
            await publisher.Publish(new NewUserRegistrationEvent(createdUser), cancellationToken);

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("Thank you for your registration. Please check your email for confirmation code."));
        }  
    }
}
