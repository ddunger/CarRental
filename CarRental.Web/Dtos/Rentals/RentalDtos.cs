namespace CarRental.Web.Dtos.Rentals
{
    public enum RentalStatus { Active, Completed, Overdue }

    public record RentalResponse(
        int Id,
        int? ReservationId,
        string? CustomerId,
        string CustomerEmail,
        string CustomerFullName,
        bool IsGuest,
        string TrackingCode,
        int VehicleId,
        string VehicleName,
        string RegistrationPlate,
        int PickupLocationId,
        string PickupLocationName,
        int DropoffLocationId,
        string DropoffLocationName,
        DateTimeOffset ActualPickupTimeUtc,
        DateTimeOffset ExpectedReturnTimeUtc,
        DateTimeOffset? ActualReturnTimeUtc,
        decimal TotalCostEuro,
        RentalStatus Status);

    public record CreateRentalRequest(
        int? ReservationId,
        string? CustomerId,
        string? GuestEmail,
        string? GuestFullName,
        string? GuestPhone,
        int? VehicleId,
        int? PickupLocationId,
        int? DropoffLocationId,
        DateTimeOffset? ExpectedReturnTimeUtc);

    public record ReturnRentalRequest(
        int? DropoffLocationId,
        DateTimeOffset? ActualReturnTimeUtc);
}