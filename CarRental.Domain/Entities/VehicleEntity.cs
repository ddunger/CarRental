using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Domain.Entities
{
    public class VehicleEntity
    {
        [Key]
        public int Id { get; set; }
        public required int ManufacturerId { get; set; } //fk 
        public required string VehicleModel { get; set; }
        [MaxLength(4)] // 4 letter code representing vehicle: category, type, transmission, fuel
        public required string AcrissCode { get; set; } = string.Empty;
        public required AcrissVehicleCategory Category { get; set; }
        public required AcrissVehicleType Type { get; set; }
        public required AcrissVehicleTransmission Transmission { get; set; }
        public required AcrissVehicleFuel Fuel { get; set; }
        public required int ManufacturingYear { get; set; }
        public required int KilometersDriven { get; set; }
        public required int EnginePowerInKw { get; set; }
        public required string RegistrationPlate { get; set; }
        public required decimal PricePerDayInEuro { get; set; }
        public required VehicleColor Color { get; set; } 

        //Navigation properties
        public ManufacturerEntity Manufacturer { get; set; } = null!;
    }
}
