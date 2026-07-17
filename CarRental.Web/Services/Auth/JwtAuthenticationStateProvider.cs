using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace CarRental.Web.Services.Auth
{
    public class JwtAuthenticationStateProvider(TokenStorageService tokenStorage)
        : AuthenticationStateProvider
    {
        private static readonly AuthenticationState Anonymous =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await tokenStorage.GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return Anonymous;

            var claims = ParseClaimsFromJwt(token);

            // Reject expired tokens so the UI doesn't pretend to be logged in
            var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (exp is not null &&
                DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)) <= DateTimeOffset.UtcNow)
            {
                return Anonymous;
            }

            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "jwt",
                nameType: "email",
                roleType: "role");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        public void NotifyUserLogout() =>
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));

        private static List<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var json = Convert.FromBase64String(PadBase64(payload));
            var pairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            var claims = new List<Claim>();
            if (pairs is null) return claims;

            foreach (var (key, value) in pairs)
            {
                // A claim appearing multiple times (e.g. roles) arrives as a JSON array
                if (value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(value.EnumerateArray()
                        .Select(e => new Claim(key, e.ToString())));
                }
                else
                {
                    claims.Add(new Claim(key, value.ToString()));
                }
            }

            return claims;
        }

        private static string PadBase64(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        }
    }
}