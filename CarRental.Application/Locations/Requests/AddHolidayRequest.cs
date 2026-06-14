namespace CarRental.Application.Locations.Requests
{
    public record AddHolidayRequest(
        DateOnly Date,
        string? HolidayName,
        bool IsClosed = true,
        TimeOnly? OpenTime = null,
        TimeOnly? CloseTime = null);
}
