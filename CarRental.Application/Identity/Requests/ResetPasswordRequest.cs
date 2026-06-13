using System.ComponentModel;

namespace CarRental.Application.Identity.Requests
{
    public record ResetPasswordRequest
    {
        [DefaultValue("")]
        public required string Email { get; init; }
        [DefaultValue("")]
        public required string Token { get; init; }
        [DefaultValue("")]
        public required string NewPassword { get; init; }
    }
}