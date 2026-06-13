namespace CarRental.Application.Identity.Requests
{
    public record Login2FARequest(string Email, string Code);

}
