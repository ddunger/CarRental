namespace CarRental.Application.Identity.Requests
{
    public record ChangePasswordRequest(string OldPassword, string NewPassword);

}
