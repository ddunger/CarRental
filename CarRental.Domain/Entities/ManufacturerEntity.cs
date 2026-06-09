using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class ManufacturerEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
