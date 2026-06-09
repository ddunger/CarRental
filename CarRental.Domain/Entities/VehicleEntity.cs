using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Domain.Entities
{
    public class VehicleEntity
    {
        [Key]
        public int Id { get; set; }
        public required int ManufacturerId { get; set; } //fk 
        public required string VehicleModel { get; set; }
        [MaxLength(4)]
        public required string AcrissCode { get; set; } = string.Empty; // 4 letter code representing vehicle: category, type, transmission, fuel
        public required int ManufacturingYear { get; set; }
        public required int KilometersDriven { get; set; }
        public required int EnginePowerInKw { get; set; }
        public required bool IsAvailable { get; set; } = true;

        //Not mapped acriss properties - calculated from AcrissCode
        [NotMapped]
        public AcrissVehicleCategory Category { get; set; }
        [NotMapped]
        public AcrissVehicleType Type { get; set; }
        [NotMapped]
        public AcrissVehicleTransmission Transmission { get; set; }
        [NotMapped]
        public AcrissVehicleFuel Fuel { get; set; }


        //Navigation properties

        public ManufacturerEntity Manufacturer { get; set; } = null!;
    }
}
