namespace CarRental.Application.Locations.Requests
{
    public record UpdateLocationRequest(
      string? Name,
      string? Address,
      double? Latitude,
      double? Longitude,
      bool? IsActive);
}
