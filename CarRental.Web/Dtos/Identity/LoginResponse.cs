namespace CarRental.Web.Dtos.Identity
{
    public record LoginResponse(
       string? AccessToken,
       string? RefreshToken,
       bool? Requires2FA,
       string? RoleName);

    public record ApiResponse<T>(T? Data);
}
