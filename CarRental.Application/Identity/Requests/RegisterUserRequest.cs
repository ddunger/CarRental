using System.ComponentModel;

namespace CarRental.Application.Identity.Requests
{
    public class RegisterUserRequest
    {
        public required string Email { get; set; }
        [DefaultValue("")]
        public required string Password { get; set; }
        [DefaultValue("")]
        public string? FirstName { get; set; }
        [DefaultValue("")]
        public string? LastName { get; set; }
    }
}

