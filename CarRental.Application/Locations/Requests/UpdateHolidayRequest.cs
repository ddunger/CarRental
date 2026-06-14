namespace CarRental.Application.Locations.Requests
{
    public record UpdateHolidayRequest(
      string? HolidayName,
      bool? IsClosed,
      TimeOnly? OpenTime,
      TimeOnly? CloseTime);
}
