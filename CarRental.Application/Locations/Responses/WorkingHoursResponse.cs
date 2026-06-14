namespace CarRental.Application.Locations.Responses
{
    public record WorkingHoursResponse(
       int Id,
       DayOfWeek DayOfWeek,
       TimeOnly OpenTime,
       TimeOnly CloseTime,
       bool IsClosed);
}
