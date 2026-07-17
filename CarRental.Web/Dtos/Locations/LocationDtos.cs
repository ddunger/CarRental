namespace CarRental.Web.Dtos.Locations
{
    public record LocationResponse(
        int Id,
        string Name,
        string Address,
        double Latitude,
        double Longitude,
        bool IsActive,
        List<WorkingHoursResponse> WorkingHours,
        List<HolidayResponse> Holidays);

    public record WorkingHoursResponse(
        int Id,
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed);

    public record HolidayResponse(
        int Id,
        DateOnly Date,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime,
        bool IsClosed,
        string? HolidayName);

    public record CreateLocationRequest(
        string Name,
        string Address,
        double Latitude,
        double Longitude);

    public record UpdateLocationRequest(
        string? Name,
        string? Address,
        double? Latitude,
        double? Longitude,
        bool? IsActive);

    public record AddWorkingHoursRequest(
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed);

    public record UpdateWorkingHoursRequest(
        TimeOnly? OpenTime,
        TimeOnly? CloseTime,
        bool? IsClosed);

    public record AddHolidayRequest(
        DateOnly Date,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime,
        bool IsClosed,
        string? HolidayName);
}