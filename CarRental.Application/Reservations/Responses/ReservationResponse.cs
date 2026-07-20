using CarRental.Domain.Enums;

namespace CarRental.Application.Reservations.Responses
{
    public record ReservationResponse(
       int Id,
       string? CustomerId,
       string CustomerEmail,        // account email OR guest email — unified for display
       string CustomerFullName,     // account name OR guest name
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
}
