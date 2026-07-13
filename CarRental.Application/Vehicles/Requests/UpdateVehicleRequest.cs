using CarRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.Vehicles.Requests
{
    public record UpdateVehicleRequest(
        [Range(1, int.MaxValue)] int? ManufacturerId,
        [StringLength(100)] string? VehicleModel,
        AcrissVehicleCategory? Category,
        AcrissVehicleType? Type,
        AcrissVehicleTransmission? Transmission,
        AcrissVehicleFuel? Fuel,
        [Range(1950, 2100)] int? ManufacturingYear,
        [Range(0, int.MaxValue)] int? KilometersDriven,
        [Range(1, 2000)] int? EnginePowerInKw,
        [StringLength(20)] string? RegistrationPlate,
        [Range(0.01, 100000)] decimal? PricePerDayInEuro,
        VehicleColor? Color);   
}
