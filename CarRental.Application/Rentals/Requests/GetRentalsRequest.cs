using CarRental.Domain.Enums;

namespace CarRental.Application.Rentals.Requests
{
    public record GetRentalsRequest(
         string? CustomerId,
         int? VehicleId,
         int? PickupLocationId,
         RentalStatus? Status,
         DateTimeOffset? PickupFromUtc,
         DateTimeOffset? PickupToUtc);
}
