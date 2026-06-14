namespace CarRental.Application.Locations.Requests
{
    public record AddWorkingHoursRequest(
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed = false);
}
