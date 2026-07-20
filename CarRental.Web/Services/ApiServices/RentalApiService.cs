using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Rentals;
using System.Net.Http.Json;

namespace CarRental.Web.Services.ApiServices
{
    public class RentalApiService(HttpClient http)
    {
        public async Task<List<RentalResponse>> GetAllAsync(
            RentalStatus? status = null, int? vehicleId = null, int? offset = null, int? limit = null)
        {
            var query = new List<string>();
            if (status.HasValue) query.Add($"status={status}");
            if (vehicleId.HasValue) query.Add($"vehicleId={vehicleId}");
            if (offset.HasValue) query.Add($"offset={offset}");
            if (limit.HasValue) query.Add($"limit={limit}");

            var url = ApiEndpoints.Rentals.GetAll + (query.Count > 0 ? "?" + string.Join("&", query) : "");

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return [];

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<RentalResponse>>>(JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<List<RentalResponse>> GetMyAsync(int? offset = null, int? limit = null)
        {
            var query = new List<string>();
            if (offset.HasValue) query.Add($"offset={offset}");
            if (limit.HasValue) query.Add($"limit={limit}");

            var url = ApiEndpoints.Rentals.GetMy + (query.Count > 0 ? "?" + string.Join("&", query) : "");

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return [];

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<RentalResponse>>>(JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<(bool Success, string? Error)> CreateAsync(CreateRentalRequest request)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Rentals.Create, request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> ReturnAsync(int id, ReturnRentalRequest request)
        {
            var response = await http.PatchAsJsonAsync(
                string.Format(ApiEndpoints.Rentals.Return, id), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

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
        public async Task<(bool Success, string? Error, RentalResponse? Created)> CreateWithResponseAsync(
            CreateRentalRequest request)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Rentals.Create, request, JsonDefaults.Options);

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                    return (false, error?.Message ?? $"Request failed ({(int)response.StatusCode}).", null);
                }
                catch
                {
                    return (false, $"Request failed ({(int)response.StatusCode}).", null);
                }
            }

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<RentalResponse>>(JsonDefaults.Options);
            return (true, null, wrapper?.Data);
        }
    }
}