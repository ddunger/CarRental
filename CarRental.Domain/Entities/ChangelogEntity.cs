using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class ChangelogEntity
    {
        [Key]
        public int Id { get; set; }
        public required string UserId { get; set; } //Fk of user who made the change
        public required DateTimeOffset ChangedAt { get; set; }
        public string? Description { get; set; }
    }
}
