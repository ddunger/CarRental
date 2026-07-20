using CarRental.Application.Vehicles.Requests;
using CarRental.Application.Vehicles.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Vehicles.Commands
{
    public record UpdateVehicleCommand(int VehicleId, UpdateVehicleRequest Request)
        : IRequest<ApplicationResult<VehicleResponse>>;

    public class UpdateVehicleCommandHandler(
        ILogger<UpdateVehicleCommandHandler> logger,
        IVehiclesRepository vehiclesRepository,
        IManufacturersRepository manufacturersRepository)
        : IRequestHandler<UpdateVehicleCommand, ApplicationResult<VehicleResponse>>
    {
        public async Task<ApplicationResult<VehicleResponse>> Handle(
            UpdateVehicleCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var vehicleResult = await vehiclesRepository.GetVehicleByIdAsync(command.VehicleId, cancellationToken);
            if (!vehicleResult.IsSuccess)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Failed to retrieve vehicle.", ResultError.Internal);

            var vehicle = vehicleResult.Value;
            if (vehicle is null)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Vehicle not found.", ResultError.NotFound);

            if (request.ManufacturerId.HasValue && request.ManufacturerId != vehicle.ManufacturerId)
            {
                var manufacturerResult = await manufacturersRepository.GetManufacturerByIdAsync(
                    request.ManufacturerId.Value, cancellationToken);

                if (!manufacturerResult.IsSuccess)
                    return ApplicationResult<VehicleResponse>.Failure(
                        "Failed to validate manufacturer.", ResultError.Internal);

                if (manufacturerResult.Value is null)
                    return ApplicationResult<VehicleResponse>.Failure(
                        "Manufacturer not found.", ResultError.Validation);

                vehicle.ManufacturerId = request.ManufacturerId.Value;
                vehicle.Manufacturer = manufacturerResult.Value;
            }

            if (request.VehicleModel is not null)
                vehicle.VehicleModel = request.VehicleModel;
            if (request.Category.HasValue)
                vehicle.Category = request.Category.Value;
            if (request.Type.HasValue)
                vehicle.Type = request.Type.Value;
            if (request.Transmission.HasValue)
                vehicle.Transmission = request.Transmission.Value;
            if (request.Fuel.HasValue)
                vehicle.Fuel = request.Fuel.Value;
            if (request.ManufacturingYear.HasValue)
                vehicle.ManufacturingYear = request.ManufacturingYear.Value;
            if (request.KilometersDriven.HasValue)
                vehicle.KilometersDriven = request.KilometersDriven.Value;
            if (request.EnginePowerInKw.HasValue)
                vehicle.EnginePowerInKw = request.EnginePowerInKw.Value;
            if (request.RegistrationPlate is not null)
                vehicle.RegistrationPlate = request.RegistrationPlate.Trim().ToUpperInvariant();
            if (request.PricePerDayInEuro.HasValue)
                vehicle.PricePerDayInEuro = request.PricePerDayInEuro.Value;
            if (request.Color.HasValue)
                vehicle.Color = request.Color.Value;

            // Keep the ACRISS code in sync with the four classification enums
            vehicle.AcrissCode = $"{vehicle.Category}{vehicle.Type}{vehicle.Transmission}{vehicle.Fuel}";

            var updatedResult = await vehiclesRepository.UpdateVehicleAsync(vehicle, cancellationToken);
            if (!updatedResult.IsSuccess || updatedResult.Value is null)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Failed to update vehicle.", ResultError.Internal);

            logger.LogInformation("Vehicle {VehicleId} updated.", vehicle.Id);
            var updated = updatedResult.Value;

            return ApplicationResult<VehicleResponse>.Success(new VehicleResponse(
                updated.Id,
                updated.ManufacturerId,
                updated.Manufacturer?.Name ?? string.Empty,
                updated.VehicleModel,
                updated.AcrissCode,
                updated.Category,
                updated.Type,
                updated.Transmission,
                updated.Fuel,
                updated.ManufacturingYear,
                updated.KilometersDriven,
                updated.EnginePowerInKw,
                updated.RegistrationPlate,
                updated.PricePerDayInEuro,
                updated.Color,
                updated.ImageData,
                updated.ImageContentType));
        }
    }
}