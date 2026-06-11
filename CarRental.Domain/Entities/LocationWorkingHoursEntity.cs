using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class LocationWorkingHoursEntity
    {
        [Key]
        public int Id { get; set; }
        public required int LocationId { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }  // .NET enum, 0=Sunday
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public bool IsClosed { get; set; } = false;    

        // Navigation
        public PickupLocation Location { get; set; } = null!;
    }
}
