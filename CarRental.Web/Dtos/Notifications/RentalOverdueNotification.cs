namespace CarRental.Web.Dtos.Notifications
{
    public record RentalOverdueNotification(
        int RentalId,
        string CustomerEmail,
        string CustomerFullName,
        string VehicleName,
        string RegistrationPlate,
        DateTimeOffset ExpectedReturnTimeUtc,
        double HoursOverdue);
}