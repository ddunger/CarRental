using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarRental.Domain.Entities
{
    public class ReservationEntity
    {
        [Key]
        public int Id { get; set; }
        public required string CustomerId {  get; set;  }
        public required int VehicleId { get; set; }
        public required int PickupLocationId { get; set; }
        public required int DropoffLocationId { get; set; }
        public required DateTimeOffset PickupTimeUtc { get; set; }
        public required DateTimeOffset ExpectedReturnTimeUtc { get; set; }
        public required decimal ExpectedCostEuro {  get; set; }
        public ReservationStatus Status { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? CancelledAtUtc { get; set; }

        //Navigation
        public UserEntity Customer { get; set; } = null!;
        public VehicleEntity Vehicle { get; set; } = null!;
        public PickupLocationEntity PickupLocation { get; set; } = null!;
        public PickupLocationEntity DropoffLocation { get; set; } = null!;
    }
}
