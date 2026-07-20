using CarRental.Application.Vehicles.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Vehicles.Queries
{
    public record GetVehicleByIdQuery(int VehicleId) : IRequest<ApplicationResult<VehicleResponse>>;

    public class GetVehicleByIdQueryHandler(
        ILogger<GetVehicleByIdQueryHandler> logger,
        IVehiclesRepository vehiclesRepository)
        : IRequestHandler<GetVehicleByIdQuery, ApplicationResult<VehicleResponse>>
    {
        public async Task<ApplicationResult<VehicleResponse>> Handle(
            GetVehicleByIdQuery query, CancellationToken cancellationToken)
        {
            var vehicleResult = await vehiclesRepository.GetVehicleByIdAsync(query.VehicleId, cancellationToken);

            if (!vehicleResult.IsSuccess)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Failed to retrieve vehicle.", ResultError.Internal);

            var vehicle = vehicleResult.Value;
            if (vehicle is null)
            {
                logger.LogInformation("Vehicle {VehicleId} not found.", query.VehicleId);
                return ApplicationResult<VehicleResponse>.Failure(
                    "Vehicle not found.", ResultError.NotFound);
            }

            return ApplicationResult<VehicleResponse>.Success(new VehicleResponse(
                vehicle.Id,
                vehicle.ManufacturerId,
                vehicle.Manufacturer?.Name ?? string.Empty,
                vehicle.VehicleModel,
                vehicle.AcrissCode,
                vehicle.Category,
                vehicle.Type,
                vehicle.Transmission,
                vehicle.Fuel,
                vehicle.ManufacturingYear,
                vehicle.KilometersDriven,
                vehicle.EnginePowerInKw,
                vehicle.RegistrationPlate,
                vehicle.PricePerDayInEuro,
                vehicle.Color,
                vehicle.ImageData,
                vehicle.ImageContentType));
        }
    }
}