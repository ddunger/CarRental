using CarRental.Domain.Enums;

namespace CarRental.Application.Vehicles.Requests
{
    public record GetVehiclesRequest(
        List<int>? ManufacturerIds,
        int? YearFrom,
        int? YearTo,
        decimal? PriceFrom,
        decimal? PriceTo,
        int? MaxKilometers,
        List<VehicleColor>? Colors,
        List<AcrissVehicleCategory>? Categories,
        List<AcrissVehicleType>? Types,
        List<AcrissVehicleTransmission>? Transmissions,
        List<AcrissVehicleFuel>? Fuels);
}
