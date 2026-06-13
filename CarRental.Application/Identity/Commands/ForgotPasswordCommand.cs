using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Application.Events;
using CarRental.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CarRental.Application.Identity.Commands
{
    public record ForgotPasswordCommand(ForgotPasswordRequest ResetRequest)
       : IRequest<ApplicationResult<StringResponse>>;

    public class ForgotPasswordCommandHandler(
        ILogger<ForgotPasswordCommandHandler> logger,
        IPublisher publisher,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ForgotPasswordCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.ResetRequest.Email);
            if (user is not null && await userManager.IsEmailConfirmedAsync(user))
            {
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
                await publisher.Publish(new ForgottenPasswordEvent(user, validToken), cancellationToken);
            }
            else
            {
                logger.LogWarning("Forgot password requested for non-existing or unconfirmed email {Email}.", command.ResetRequest.Email);
            }

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("Please check your email for instructions on resetting your password."));
        }
    }
}
