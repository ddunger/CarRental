namespace CarRental.Application.Common.Results
{
    public record TokenResult(string AccessToken, string RefreshToken, string? RoleName = null);

}
