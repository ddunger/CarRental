using CarRental.Application.Common.Responses;
using CarRental.Domain.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CarRental.Application.Identity.Commands
{
    public record ResetPasswordCommand(ResetPasswordRequest ResetPasswordDto)
       : IRequest<ApplicationResult<StringResponse>>;

    public class ResetPasswordCommandHandler(
        ILogger<ResetPasswordCommandHandler> logger,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ResetPasswordCommand, ApplicationResult<StringResponse>>
    {
        public async Task<ApplicationResult<StringResponse>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var dto = command.ResetPasswordDto;

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                logger.LogWarning("Invalid reset password attempt for email {Email}.", dto.Email);
                return ApplicationResult<StringResponse>.Failure("Invalid password reset request.", ResultError.Validation);
            }

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            }
            catch (FormatException)
            {
                return ApplicationResult<StringResponse>.Failure("Invalid password reset request.", ResultError.Validation);
            }

            var result = await userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogInformation("Password reset rejected for user {UserId}: {Errors}.", user.Id, errors);
                return ApplicationResult<StringResponse>.Failure(errors, ResultError.Validation);
            }

            return ApplicationResult<StringResponse>.Success(
                new StringResponse("Your password has been reset successfully."));
        }
    }
}