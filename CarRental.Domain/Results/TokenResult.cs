namespace CarRental.Domain.Results
{
    public record TokenResult(string AccessToken, string RefreshToken, string? RoleName = null);

}
