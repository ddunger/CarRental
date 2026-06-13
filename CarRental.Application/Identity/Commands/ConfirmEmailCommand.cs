using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Identity.Commands
{
    public record ConfirmEmailCommand(ConfirmEmailRequest ConfirmEmailDto)
        : IRequest<Result<StringResponse>>;

    public class ConfirmEmailCommandHandler(
        ILogger<ConfirmEmailCommandHandler> logger,
        UserManager<UserEntity> userManager)
        : IRequestHandler<ConfirmEmailCommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(command.ConfirmEmailDto.Email);
            if (user is null)
            {
                logger.LogError("User with email {Email} not found.", command.ConfirmEmailDto.Email);
                return Result<StringResponse>.Failure("Invalid data provided.", ResultError.NotFound); 
            }

            var result = await userManager.ConfirmEmailAsync(user, command.ConfirmEmailDto.Code);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Error confirming email {Email}: {Errors}", command.ConfirmEmailDto.Email, errors);
                return Result<StringResponse>.Failure(errors, ResultError.Validation);
            }

            return Result<StringResponse>.Success(
                new StringResponse("Email confirmed successfully, you can proceed to login."));
        }
    }
}