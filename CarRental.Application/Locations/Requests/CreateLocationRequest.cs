namespace CarRental.Application.Locations.Requests
{
    public record CreateLocationRequest(
        string Name,
        string Address,
        double Latitude,
        double Longitude);
}
