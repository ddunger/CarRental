using CarRental.API.Extensions;
using CarRental.Application.Users.Commands;
using CarRental.Application.Users.Queries;
using CarRental.Application.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Returns a paginated list of all users. Admin and Manager access only.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsersAsync(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllUsersQuery(offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single user by ID. Admin and Manager access only.
        /// </remarks>
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserByIdAsync(
            [FromRoute] string userId)
        {
            var result = await sender.Send(new GetUserByIdQuery(userId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Disables 2FA for a specific user. Admin access only.
        /// </remarks>
        [HttpPatch("disable-2fa/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable2FAAsync(
            [FromRoute] string userId)
        {

            var result = await sender.Send(new AdminDisable2faCommand(userId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Deactivates a user account, preventing further logins. Admin access only.
        /// </remarks>
        [HttpPatch("deactivate/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUserAsync(
            [FromRoute] string userId)
        {

            var result = await sender.Send(new AdminDeactivateUserCommand(userId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates users first and last names.
        /// </remarks>
        [HttpPatch("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(
            [FromRoute] string userId,
            [FromBody] UpdateUserRequest request)
        {
            var result = await sender.Send(new UpdateUserCommand(userId, request));
            return result.ToActionResult(this);
        }

    }
}
