using System.ComponentModel;

namespace CarRental.Application.Identity.Requests
{
    public class LoginUserRequest
    {
        [DefaultValue("")]
        public required string Email { get; set; }
        [DefaultValue("")]
        public required string Password { get; set; }

    }
}