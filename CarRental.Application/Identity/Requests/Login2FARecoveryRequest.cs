namespace CarRental.Application.Identity.Requests
{
    public record Login2FARecoveryRequest(string Email, string RecoveryCode);

}
