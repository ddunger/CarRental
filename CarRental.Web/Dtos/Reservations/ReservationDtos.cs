namespace CarRental.Web.Dtos.Reservations
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled, NoShow, Converted }

    public record ReservationResponse(
        int Id,
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
        DateTimeOffset PickupTimeUtc,
        DateTimeOffset ExpectedReturnTimeUtc,
        decimal ExpectedCostEuro,
        ReservationStatus Status,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? CancelledAtUtc);

    public record GetReservationsRequest(
        string? CustomerId,
        int? VehicleId,
        int? PickupLocationId,
        ReservationStatus? Status,
        DateTimeOffset? PickupFromUtc,
        DateTimeOffset? PickupToUtc);

    public record CreateReservationRequest(
        string? CustomerId,
        string? GuestEmail,
        string? GuestFullName,
        string? GuestPhone,
        int VehicleId,
        int PickupLocationId,
        int DropoffLocationId,
        DateTimeOffset PickupTimeUtc,
        DateTimeOffset ExpectedReturnTimeUtc);
}