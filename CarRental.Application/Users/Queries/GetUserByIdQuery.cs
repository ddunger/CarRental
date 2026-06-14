using CarRental.Application.Users.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Users.Queries
{
    public record GetUserByIdQuery(string UserId) : IRequest<ApplicationResult<UserResponse>>;

    public class GetUserByIdQueryHandler(
        ILogger<GetUserByIdQueryHandler> logger,
        UserManager<UserEntity> userManager)
        : IRequestHandler<GetUserByIdQuery, ApplicationResult<UserResponse>>
    {
        public async Task<ApplicationResult<UserResponse>> Handle(
            GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(query.UserId);
            if (user is null)
            {
                logger.LogWarning("User {UserId} not found.", query.UserId);
                return ApplicationResult<UserResponse>.Failure("User not found.", ResultError.NotFound);
            }

            var roles = await userManager.GetRolesAsync(user);
            return ApplicationResult<UserResponse>.Success(new UserResponse(
                user.Id,
                user.Email!,
                roles.FirstOrDefault(),
                user.FirstName,
                user.LastName,
                user.IsActive,
                user.TwoFactorEnabled,
                user.CreatedAt,
                user.UpdatedAt));
        }
    }
}