using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Tracking;
using System.Net.Http.Json;

namespace CarRental.Web.Services.ApiServices
{
    public class TrackingApiService(HttpClient http)
    {
        public async Task<TrackingResponse?> GetAsync(string trackingCode)
        {
            var response = await http.GetAsync(
                string.Format(ApiEndpoints.Tracking.Get, Uri.EscapeDataString(trackingCode)));
            if (!response.IsSuccessStatusCode)
                return null;

            var wrapper = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TrackingResponse>>(JsonDefaults.Options);
            return wrapper?.Data;
        }

        public async Task<(bool Success, string? Error)> CancelAsync(string trackingCode)
        {
            var response = await http.PatchAsync(
                string.Format(ApiEndpoints.Tracking.Cancel, Uri.EscapeDataString(trackingCode)), null);

            if (response.IsSuccessStatusCode)
                return (true, null);

            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, error?.Message ?? $"Request failed ({(int)response.StatusCode}).");
            }
            catch
            {
                return (false, $"Request failed ({(int)response.StatusCode}).");
            }
        }
    }
}