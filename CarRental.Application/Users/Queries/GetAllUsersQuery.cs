using CarRental.Application.Users.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Users.Queries
{
    public record GetAllUsersQuery(int? Offset, int? Limit) : IRequest<ApplicationResult<IEnumerable<UserResponse>>>;


    public class GetAllUsersQueryHandler(
        ILogger<GetAllUsersQueryHandler> logger,
        UserManager<UserEntity> userManager)
        : IRequestHandler<GetAllUsersQuery, ApplicationResult<IEnumerable<UserResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<UserResponse>>> Handle(
            GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<UserResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);
            if (query.Limit.HasValue && query.Limit < 1)
                return ApplicationResult<IEnumerable<UserResponse>>.Failure(
                    "Limit must be 1 or greater.", ResultError.Validation);

            var usersQuery = userManager.Users.AsQueryable();
            if (query.Offset.HasValue && query.Limit.HasValue)
                usersQuery = usersQuery.Skip(query.Offset.Value).Take(query.Limit.Value);

            var users = await usersQuery.ToListAsync(cancellationToken);

            var responses = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                responses.Add(new UserResponse(
                    user.Id,
                    user.Email!,
                    roles.FirstOrDefault(),
                    user.FirstName,
                    user.LastName,
                    user.IsActive,
                    user.TwoFactorEnabled,
                    user.CreatedAt,
                    user.CreatedAt));
            }

            logger.LogInformation("Fetched {Count} user(s).", responses.Count);
            return ApplicationResult<IEnumerable<UserResponse>>.Success(responses);
        }
    }
}