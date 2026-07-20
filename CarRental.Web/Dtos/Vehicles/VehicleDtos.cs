namespace CarRental.Web.Dtos.Vehicles
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
        VehicleColor Color,
        byte[]? ImageData,
        string? ImageContentType);

    public record CreateVehicleRequest(
        int ManufacturerId,
        string VehicleModel,
        AcrissVehicleCategory Category,
        AcrissVehicleType Type,
        AcrissVehicleTransmission Transmission,
        AcrissVehicleFuel Fuel,
        int ManufacturingYear,
        int KilometersDriven,
        int EnginePowerInKw,
        string RegistrationPlate,
        decimal PricePerDayInEuro,
        VehicleColor Color,
        byte[]? ImageData,
        string? ImageContentType);

    public record UpdateVehicleRequest(
        int? ManufacturerId,
        string? VehicleModel,
        AcrissVehicleCategory? Category,
        AcrissVehicleType? Type,
        AcrissVehicleTransmission? Transmission,
        AcrissVehicleFuel? Fuel,
        int? ManufacturingYear,
        int? KilometersDriven,
        int? EnginePowerInKw,
        string? RegistrationPlate,
        decimal? PricePerDayInEuro,
        VehicleColor? Color,
        byte[]? ImageData,
        string? ImageContentType);
}

