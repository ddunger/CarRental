using CarRental.Application.Common.Responses;
using CarRental.Application.Common.Results;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Entities;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
namespace CarRental.Application.Identity.Commands
{
    public record RegisterNewUserCommand(RegisterUserRequest request) : IRequest<Result<StringResponse>>;


    public class RegisterNewUserCommandHandler(
        ILogger<RegisterNewUserCommandHandler> logger,
        IPublisher publisher,
        UserManager<UserEntity> usermanager)
        : IRequestHandler<RegisterNewUserCommand, Result<StringResponse>>
    {
        public async Task<Result<StringResponse>> Handle(RegisterNewUserCommand command, CancellationToken cancellationToken)
        {


            //TODO SVE


            //Final response
            return Result<StringResponse>.Success(
            new StringResponse("Thank you for your registration. Please check your email for confirmation code.")); 
        }  
    }
}
