using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class LocationHolidayEntity
    {
        [Key]
        public int Id { get; set; }
        public required int LocationId { get; set; }
        public required DateOnly Date { get; set; }
        public TimeOnly? OpenTime { get; set; }            // null = closed 
        public TimeOnly? CloseTime { get; set; }
        public bool IsClosed { get; set; } = true;
        public string? HolidayName { get; set; }           

        // Navigation
        public PickupLocation Location { get; set; } = null!;
    }
}
