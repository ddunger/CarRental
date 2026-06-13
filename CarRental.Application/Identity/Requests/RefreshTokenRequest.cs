using System.ComponentModel;

namespace CarRental.Application.Identity.Requests
{
    public class RefreshTokenRequest
    {
        [DefaultValue("")]
        public required string RefreshToken { get; set; }
    }
}
