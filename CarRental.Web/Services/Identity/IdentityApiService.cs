using CarRental.Web.Dtos.Identity;
using System.Net.Http.Json;

namespace CarRental.Web.Services.Identity
{
    public class IdentityApiService(HttpClient http, Auth.TokenStorageService tokenStorage)
    {
        public async Task<(bool Success, bool Requires2FA, string? Error)> LoginAsync(string email, string password)
        {
            var response = await http.PostAsJsonAsync("api/identity/login/web", new LoginRequest(email, password));

            if (!response.IsSuccessStatusCode)
                return (false, false, "Invalid credentials.");

            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
            var data = wrapper?.Data;

            if (data?.Requires2FA == true)
                return (false, true, null);

            if (string.IsNullOrEmpty(data?.AccessToken) || string.IsNullOrEmpty(data.RefreshToken))
                return (false, false, "Unexpected response from server.");

            await tokenStorage.SetTokensAsync(data.AccessToken, data.RefreshToken);
            return (true, false, null);
        }

        public async Task LogoutAsync()
        {
            try { await http.PostAsync("api/identity/logout/web", null); }
            catch { /* best effort — clear local state regardless */ }
            await tokenStorage.ClearTokensAsync();
        }
    }
}