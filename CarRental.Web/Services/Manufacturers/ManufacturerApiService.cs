using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Manufacturers;
using System.Net.Http.Json;

namespace CarRental.Web.Services.Manufacturers
{
    public class ManufacturerApiService(HttpClient http)
    {
        public async Task<List<ManufacturerResponse>> GetAllAsync(int? offset = null, int? limit = null)
        {
            var url = ApiEndpoints.Manufacturers.GetAll;
            if (offset.HasValue || limit.HasValue)
                url += $"?offset={offset ?? 0}&limit={limit ?? 50}";

            var wrapper = await http.GetFromJsonAsync<ApiResponse<List<ManufacturerResponse>>>(url);
            return wrapper?.Data ?? [];
        }

        public async Task<ManufacturerResponse?> GetByIdAsync(int id)
        {
            var response = await http.GetAsync(string.Format(ApiEndpoints.Manufacturers.GetById, id));
            if (!response.IsSuccessStatusCode)
                return null;

            var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<ManufacturerResponse>>();
            return wrapper?.Data;
        }

        public async Task<(bool Success, string? Error)> CreateAsync(string name)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Manufacturers.Create, new CreateManufacturerRequest(name));
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(int id, string name)
        {
            var response = await http.PatchAsJsonAsync(
                string.Format(ApiEndpoints.Manufacturers.Update, id), new UpdateManufacturerRequest(name));
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var response = await http.DeleteAsync(string.Format(ApiEndpoints.Manufacturers.Delete, id));
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

    public record ApiErrorResponse(string? Message);
}