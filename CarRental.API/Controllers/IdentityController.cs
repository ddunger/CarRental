using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using CarRental.Application.Identity.Requests;
using MediatR;

namespace CarRental.API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/identity")] //custom route naming for clarity

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Register endpoint which requires a DTO, email and password are mandatory, while first and last names are optional. 
        /// After registration, confirmation email with a 6-digit code is sent to the user. Confirmation is required for login to be available.
        /// .NET Identity requires that passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least six characters long.
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            var result = await sender.Send(new RegisterNewUserCommand(request));
            return result.ToActionResult(this);
        }
    }
}
