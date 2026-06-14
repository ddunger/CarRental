namespace CarRental.Application.Locations.Requests
{
    public record UpdateWorkingHoursRequest(
         TimeOnly? OpenTime,
         TimeOnly? CloseTime,
         bool? IsClosed);
}
