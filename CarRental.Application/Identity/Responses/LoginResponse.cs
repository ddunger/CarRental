using System.Text.Json.Serialization;

namespace CarRental.Application.Identity.Responses
{
    public record LoginResponse(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? AccessToken = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? RefreshToken = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        bool? Requires2FA = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? RoleName = null);
}
