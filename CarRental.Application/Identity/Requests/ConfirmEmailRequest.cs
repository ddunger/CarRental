using System.ComponentModel;

namespace CarRental.Application.Identity.Requests
{
    public class ConfirmEmailRequest
    {
        [DefaultValue("")]
        public required string Email { get; set; }
        [DefaultValue("")]
        public required string Code { get; set; }
    }
}
