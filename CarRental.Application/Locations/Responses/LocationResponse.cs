namespace CarRental.Application.Locations.Responses
{
    public record LocationResponse(
       int Id,
       string Name,
       string Address,
       double Latitude,
       double Longitude,
       bool IsActive,
       IEnumerable<WorkingHoursResponse> WorkingHours,
       IEnumerable<HolidayResponse> Holidays);
}
