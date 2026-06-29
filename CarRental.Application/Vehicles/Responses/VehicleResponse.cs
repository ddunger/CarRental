using CarRental.Domain.Enums;

namespace CarRental.Application.Vehicles.Responses
{
    public record VehicleResponse(
       int Id,
       int ManufacturerId,
       string ManufacturerName,
       string VehicleModel,
       string AcrissCode,
       AcrissVehicleCategory Category,
       AcrissVehicleType Type,
       AcrissVehicleTransmission Transmission,
       AcrissVehicleFuel Fuel,
       int ManufacturingYear,
       int KilometersDriven,
       int EnginePowerInKw,
       string RegistrationPlate,
       decimal PricePerDayInEuro,
       VehicleColor Color);
}
