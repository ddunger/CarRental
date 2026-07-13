using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.Reservations.Requests
{
    public record CreateReservationRequest(
           string? CustomerId,                       // only honored for Admin/Manager
           [Range(1, int.MaxValue)] int VehicleId,
           [Range(1, int.MaxValue)] int PickupLocationId,
           [Range(1, int.MaxValue)] int DropoffLocationId,
           [Required] DateTimeOffset PickupTimeUtc,
           [Required] DateTimeOffset ExpectedReturnTimeUtc);
}
