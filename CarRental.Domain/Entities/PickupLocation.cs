using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class PickupLocation
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<LocationWorkingHoursEntity> WorkingHours { get; set; } = [];
        public ICollection<LocationHolidayEntity> Holidays { get; set; } = [];
    }
}
