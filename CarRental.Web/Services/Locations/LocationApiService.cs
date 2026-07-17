using CarRental.Web.Constants;
using CarRental.Web.Dtos.Identity;
using CarRental.Web.Dtos.Locations;
using CarRental.Web.Services.Manufacturers;
using System.Net.Http.Json;

namespace CarRental.Web.Services.Locations
{
    public class LocationApiService(HttpClient http)
    {
        public async Task<List<LocationResponse>> GetAllAsync()
        {
            var wrapper = await http.GetFromJsonAsync<ApiResponse<List<LocationResponse>>>(
                ApiEndpoints.Locations.GetAll, JsonDefaults.Options);
            return wrapper?.Data ?? [];
        }

        public async Task<LocationResponse?> GetByIdAsync(int id)
        {
            var response = await http.GetAsync(string.Format(ApiEndpoints.Locations.GetById, id));
            if (!response.IsSuccessStatusCode)
                return null;

            var wrapper = await response.Content
                .ReadFromJsonAsync<ApiResponse<LocationResponse>>(JsonDefaults.Options);
            return wrapper?.Data;
        }

        public async Task<(bool Success, string? Error)> CreateAsync(CreateLocationRequest request)
        {
            var response = await http.PostAsJsonAsync(
                ApiEndpoints.Locations.Create, request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateLocationRequest request)
        {
            var response = await http.PatchAsJsonAsync(
                string.Format(ApiEndpoints.Locations.Update, id), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var response = await http.DeleteAsync(string.Format(ApiEndpoints.Locations.Delete, id));
            return await ToResultAsync(response);
        }

        // --- Working hours ---

        public async Task<(bool Success, string? Error)> AddWorkingHoursAsync(int locationId, AddWorkingHoursRequest request)
        {
            var response = await http.PostAsJsonAsync(
                string.Format(ApiEndpoints.Locations.AddWorkingHours, locationId), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> UpdateWorkingHoursAsync(int locationId, int hoursId, UpdateWorkingHoursRequest request)
        {
            var response = await http.PatchAsJsonAsync(
                string.Format(ApiEndpoints.Locations.UpdateWorkingHours, locationId, hoursId), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> DeleteWorkingHoursAsync(int locationId, int hoursId)
        {
            var response = await http.DeleteAsync(
                string.Format(ApiEndpoints.Locations.DeleteWorkingHours, locationId, hoursId));
            return await ToResultAsync(response);
        }

        // --- Holidays ---

        public async Task<(bool Success, string? Error)> AddHolidayAsync(int locationId, AddHolidayRequest request)
        {
            var response = await http.PostAsJsonAsync(
                string.Format(ApiEndpoints.Locations.AddHoliday, locationId), request, JsonDefaults.Options);
            return await ToResultAsync(response);
        }

        public async Task<(bool Success, string? Error)> DeleteHolidayAsync(int locationId, int holidayId)
        {
            var response = await http.DeleteAsync(
                string.Format(ApiEndpoints.Locations.DeleteHoliday, locationId, holidayId));
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