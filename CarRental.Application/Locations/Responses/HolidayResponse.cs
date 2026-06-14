namespace CarRental.Application.Locations.Responses
{
    public record HolidayResponse(
     int Id,
     DateOnly Date,
     string? HolidayName,
     bool IsClosed,
     TimeOnly? OpenTime,
     TimeOnly? CloseTime);
}
