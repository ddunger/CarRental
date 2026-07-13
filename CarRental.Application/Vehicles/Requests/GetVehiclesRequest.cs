using CarRental.Domain.Enums;

namespace CarRental.Application.Vehicles.Requests
{
    public record GetVehiclesRequest(
         int? ManufacturerId,
         int? YearFrom,
         int? YearTo,
         decimal? PriceFrom,
         decimal? PriceTo,
         int? MaxKilometers,
         VehicleColor? Color,
         AcrissVehicleCategory? Category,
         AcrissVehicleType? Type,
         AcrissVehicleTransmission? Transmission,
         AcrissVehicleFuel? Fuel);
}
