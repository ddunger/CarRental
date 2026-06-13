using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
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
       : IRequest<ApplicationResult<StringResponse>>;

    public class ResendConfirmationCommandHandler(
        ILogger<ResendConfirmationCommandHandler> logger,
        IPublisher publisher,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ResendConfirmationCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(
            ResendConfirmationCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.ResendRequest.Email);
            if (user is not null && !await userManager.IsEmailConfirmedAsync(user))
            {
                logger.LogInformation("Resending confirmation email for user {UserId}.", user.Id);
                await publisher.Publish(new NewUserRegistrationEvent(user), cancellationToken);
            }

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("Thank you for your registration. Please check your email for confirmation code."));
        }
    }

}
