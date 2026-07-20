using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class RentalEntity
    {
        [Key]
        public int Id { get; set; }
        public int? ReservationId { get; set; }               
        public string? CustomerId { get; set; }
        public required int VehicleId { get; set; }
        public required int PickupLocationId { get; set; }
        public required int DropoffLocationId { get; set; }
        public required DateTimeOffset ActualPickupTimeUtc { get; set; }
        public required DateTimeOffset ExpectedReturnTimeUtc { get; set; }
        public DateTimeOffset? ActualReturnTimeUtc { get; set; }   
        public required decimal TotalCostEuro { get; set; }
        public RentalStatus Status { get; set; }               // Active, Completed, Overdue

        [StringLength(256)]
        public string? GuestEmail { get; set; }

        [StringLength(100)]
        public string? GuestFullName { get; set; }

        [StringLength(30)]
        public string? GuestPhone { get; set; }

        [StringLength(64)]
        public string TrackingCode { get; set; } = default!;


        // Navigation
        public ReservationEntity? Reservation { get; set; }
        public UserEntity? Customer { get; set; } = null!;
        public VehicleEntity Vehicle { get; set; } = null!;
        public PickupLocationEntity PickupLocation { get; set; } = null!;
        public PickupLocationEntity DropoffLocation { get; set; } = null!;
    }
}
