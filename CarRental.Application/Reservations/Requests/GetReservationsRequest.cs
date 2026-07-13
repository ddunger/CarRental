using CarRental.Domain.Enums;

namespace CarRental.Application.Reservations.Requests
{
    public record GetReservationsRequest(
         string? CustomerId,
         int? VehicleId,
         int? PickupLocationId,
         ReservationStatus? Status,
         DateTimeOffset? PickupFromUtc,
         DateTimeOffset? PickupToUtc);
}
