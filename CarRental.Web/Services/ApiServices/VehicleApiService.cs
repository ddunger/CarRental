using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Vehicles;
using System.Net.Http.Json;

namespace CarRental.Web.Services.ApiServices
{
    public class VehicleApiService(HttpClient http)
    {
        public async Task<List<VehicleResponse>> SearchAsync(
            GetVehiclesRequest filter, int? offset = null, int? limit = null)
        {
            var url = ApiEndpoints.Vehicles.Search;
            var paging = new List<string>();
            if (offset.HasValue) paging.Add($"offset={offset}");
            if (limit.HasValue) paging.Add($"limit={limit}");
            if (paging.Count > 0) url += "?" + string.Join("&", paging);

            var response = await http.PostAsJsonAsync(url, filter, JsonDefaults.Options);
            if (!response.IsSuccessStatusCode)
                return [];

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<VehicleResponse>>>(JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id)
        {
            var response = await http.GetAsync(string.Format(ApiEndpoints.Vehicles.GetById, id));
            if (!response.IsSuccessStatusCode)
                return null;

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<VehicleResponse>>(JsonDefaults.Options);
            return wrapper?.Data;
        }

        public async Task<(bool Success, string? Error)> CreateAsync(CreateVehicleRequest request)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Vehicles.Create, request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateVehicleRequest request)
        {
            var response = await http.PatchAsJsonAsync(
                string.Format(ApiEndpoints.Vehicles.Update, id), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var response = await http.DeleteAsync(string.Format(ApiEndpoints.Vehicles.Delete, id));
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
    }
}