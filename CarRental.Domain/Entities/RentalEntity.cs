using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class RentalEntity
    {
        [Key]
        public int Id { get; set; }
        public int? ReservationId { get; set; }               
        public required string CustomerId { get; set; }
        public required int VehicleId { get; set; }
        public required int PickupLocationId { get; set; }
        public required int DropoffLocationId { get; set; }
        public required DateTimeOffset ActualPickupTimeUtc { get; set; }
        public required DateTimeOffset ExpectedReturnTimeUtc { get; set; }
        public DateTimeOffset? ActualReturnTimeUtc { get; set; }   
        public required decimal TotalCostEuro { get; set; }
        public RentalStatus Status { get; set; }               // Active, Completed, Overdue

        // Navigation
        public ReservationEntity? Reservation { get; set; }
        public UserEntity Customer { get; set; } = null!;
        public VehicleEntity Vehicle { get; set; } = null!;
        public PickupLocationEntity PickupLocation { get; set; } = null!;
        public PickupLocationEntity DropoffLocation { get; set; } = null!;
    }
}
