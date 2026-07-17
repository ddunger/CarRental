using Microsoft.JSInterop;

namespace CarRental.Web.Services.Auth
{
    public class TokenStorageService(IJSRuntime js)
    {
        private const string AccessTokenKey = "carrental_access_token";
        private const string RefreshTokenKey = "carrental_refresh_token";

        public async ValueTask<string?> GetAccessTokenAsync() =>
            await js.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);

        public async ValueTask<string?> GetRefreshTokenAsync() =>
            await js.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);

        public async Task SetTokensAsync(string accessToken, string refreshToken)
        {
            await js.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, accessToken);
            await js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
        }

        public async Task ClearTokensAsync()
        {
            await js.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
            await js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        }
    }
}