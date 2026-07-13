using CarRental.Domain.Enums;

namespace CarRental.Application.Rentals.Responses
{
    public record RentalResponse(
        int Id,
        int? ReservationId,
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
        DateTimeOffset ActualPickupTimeUtc,
        DateTimeOffset ExpectedReturnTimeUtc,
        DateTimeOffset? ActualReturnTimeUtc,
        decimal TotalCostEuro,
        RentalStatus Status);
}