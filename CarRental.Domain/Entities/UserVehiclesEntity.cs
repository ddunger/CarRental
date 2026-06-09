using System.ComponentModel.DataAnnotations;


namespace CarRental.Domain.Entities
{
    public class UserVehiclesEntity
    {
        [Key]
        public int Id { get; set; }
        public required string UserId { get; set; } //fk to UserEntity
        public required int VehicleId { get; set; } //fk to VehicleEntity
        public DateTimeOffset RentedAtUtc { get; set; }
        public DateTimeOffset? ReturnedAtUtc { get; set; } = null;
        public DateTimeOffset? ExpectedReturnTimeUtc { get; set; } = null;

    }
}
