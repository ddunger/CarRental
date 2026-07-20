namespace CarRental.Application.Rentals.Requests
{
    public record CreateRentalRequest(
        // Option A: convert an existing reservation (all other fields ignored/derived)
        int? ReservationId,

        // Option B: walk-in — customer is EITHER a registered account (CustomerId)
        // OR a guest (GuestEmail + GuestFullName [+ GuestPhone])
        string? CustomerId,
        string? GuestEmail,
        string? GuestFullName,
        string? GuestPhone,
        int? VehicleId,
        int? PickupLocationId,
        int? DropoffLocationId,
        DateTimeOffset? ExpectedReturnTimeUtc);
}
