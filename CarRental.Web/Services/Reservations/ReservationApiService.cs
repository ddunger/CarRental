using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Reservations;
using CarRental.Web.Services.Manufacturers;
using System.Net.Http.Json;

namespace CarRental.Web.Services.Reservations
{
    public class ReservationApiService(HttpClient http)
    {
        public async Task<List<ReservationResponse>> GetAllAsync(
            GetReservationsRequest filter, int? offset = null, int? limit = null)
        {
            var query = new List<string>();
            if (filter.CustomerId is not null) query.Add($"customerId={Uri.EscapeDataString(filter.CustomerId)}");
            if (filter.VehicleId.HasValue) query.Add($"vehicleId={filter.VehicleId}");
            if (filter.PickupLocationId.HasValue) query.Add($"pickupLocationId={filter.PickupLocationId}");
            if (filter.Status.HasValue) query.Add($"status={filter.Status}");
            if (filter.PickupFromUtc.HasValue) query.Add($"pickupFromUtc={Uri.EscapeDataString(filter.PickupFromUtc.Value.ToString("O"))}");
            if (filter.PickupToUtc.HasValue) query.Add($"pickupToUtc={Uri.EscapeDataString(filter.PickupToUtc.Value.ToString("O"))}");
            if (offset.HasValue) query.Add($"offset={offset}");
            if (limit.HasValue) query.Add($"limit={limit}");

            var url = ApiEndpoints.Reservations.GetAll + (query.Count > 0 ? "?" + string.Join("&", query) : "");

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return [];

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<ReservationResponse>>>(JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<List<ReservationResponse>> GetMyAsync(int? offset = null, int? limit = null)
        {
            var query = new List<string>();
            if (offset.HasValue) query.Add($"offset={offset}");
            if (limit.HasValue) query.Add($"limit={limit}");

            var url = ApiEndpoints.Reservations.GetMy + (query.Count > 0 ? "?" + string.Join("&", query) : "");

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return [];

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<ReservationResponse>>>(JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<(bool Success, string? Error)> CreateAsync(CreateReservationRequest request)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Reservations.Create, request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> ConfirmAsync(int id) =>
            await ToResultAsync(await http.PatchAsync(string.Format(ApiEndpoints.Reservations.Confirm, id), null));

        public async Task<(bool Success, string? Error)> CancelAsync(int id) =>
            await ToResultAsync(await http.PatchAsync(string.Format(ApiEndpoints.Reservations.Cancel, id), null));

        public async Task<(bool Success, string? Error)> MarkNoShowAsync(int id) =>
            await ToResultAsync(await http.PatchAsync(string.Format(ApiEndpoints.Reservations.NoShow, id), null));

        private static async Task<(bool, string?)> ToResultAsync(HttpResponseMessage response)
        {
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