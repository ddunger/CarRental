using CarRental.Domain.Enums;

namespace CarRental.Application.Reservations.Responses
{
    public record ReservationResponse(
        int Id,
        string CustomerId,
        string CustomerEmail,
        string CustomerFullName,
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
