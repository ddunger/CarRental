namespace CarRental.Application.Identity.Requests
{
    public record Disable2FARecoveryRequest(string Email, string RecoveryCode);

}
