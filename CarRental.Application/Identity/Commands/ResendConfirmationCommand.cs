using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Application.Events;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.Identity.Commands
{
    public record ResendConfirmationCommand(ResendConfirmationEmailRequest ResendRequest)
       : IRequest<Result<StringResponse>>;

    public class ResendConfirmationCommandHandler(
        ILogger<ResendConfirmationCommandHandler> logger,
        IPublisher publisher,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ResendConfirmationCommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(
            ResendConfirmationCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.ResendRequest.Email);
            if (user is not null && !await userManager.IsEmailConfirmedAsync(user))
            {
                logger.LogInformation("Resending confirmation email for user {UserId}.", user.Id);
                await publisher.Publish(new NewUserRegistrationEvent(user), cancellationToken);
            }

            return Result<StringResponse>.Success(
                new StringResponse("Thank you for your registration. Please check your email for confirmation code."));
        }
    }

}
