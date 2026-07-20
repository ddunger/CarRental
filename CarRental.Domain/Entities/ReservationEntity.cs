using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class ReservationEntity
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerId {  get; set;  }
        public required int VehicleId { get; set; }
        public required int PickupLocationId { get; set; }
        public required int DropoffLocationId { get; set; }
        public required DateTimeOffset PickupTimeUtc { get; set; }
        public required DateTimeOffset ExpectedReturnTimeUtc { get; set; }
        public required decimal ExpectedCostEuro {  get; set; }
        public ReservationStatus Status { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? CancelledAtUtc { get; set; }

        // Guest (non-registered) customer
        [StringLength(256)]
        public string? GuestEmail { get; set; }

        [StringLength(100)]
        public string? GuestFullName { get; set; }

        [StringLength(30)]
        public string? GuestPhone { get; set; }

        [StringLength(64)]
        public string TrackingCode { get; set; } = default!;

        //Navigation
        public UserEntity? Customer { get; set; }
        public VehicleEntity Vehicle { get; set; } = null!;
        public PickupLocationEntity PickupLocation { get; set; } = null!;
        public PickupLocationEntity DropoffLocation { get; set; } = null!;
    }
}
