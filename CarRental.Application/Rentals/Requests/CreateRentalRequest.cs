namespace CarRental.Application.Rentals.Requests
{
    public record CreateRentalRequest(
         // Option A: convert an existing reservation (other fields below are ignored/derived)
         int? ReservationId,

         // Option B: walk-in rental (all required when ReservationId is null)
         string? CustomerId,
         int? VehicleId,
         int? PickupLocationId,
         int? DropoffLocationId,
         DateTimeOffset? ExpectedReturnTimeUtc);
}
