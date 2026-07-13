using CarRental.Application.Vehicles.Requests;
using CarRental.Application.Vehicles.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Vehicles.Commands
{
    public record CreateVehicleCommand(CreateVehicleRequest Request) : IRequest<ApplicationResult<VehicleResponse>>;

    public class CreateVehicleCommandHandler(
        ILogger<CreateVehicleCommandHandler> logger,
        IVehiclesRepository vehiclesRepository,
        IManufacturersRepository manufacturersRepository)
        : IRequestHandler<CreateVehicleCommand, ApplicationResult<VehicleResponse>>
    {
        public async Task<ApplicationResult<VehicleResponse>> Handle(
            CreateVehicleCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var manufacturerResult = await manufacturersRepository.GetManufacturerByIdAsync(
                request.ManufacturerId, cancellationToken);

            if (!manufacturerResult.IsSuccess)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Failed to validate manufacturer.", ResultError.Internal);

            if (manufacturerResult.Value is null)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Manufacturer not found.", ResultError.Validation);

            var vehicle = new VehicleEntity
            {
                ManufacturerId = request.ManufacturerId,
                VehicleModel = request.VehicleModel,
                AcrissCode = $"{request.Category}{request.Type}{request.Transmission}{request.Fuel}",
                Category = request.Category,
                Type = request.Type,
                Transmission = request.Transmission,
                Fuel = request.Fuel,
                ManufacturingYear = request.ManufacturingYear,
                KilometersDriven = request.KilometersDriven,
                EnginePowerInKw = request.EnginePowerInKw,
                RegistrationPlate = request.RegistrationPlate.Trim().ToUpperInvariant(),
                PricePerDayInEuro = request.PricePerDayInEuro,
                Color = request.Color
            };

            var createdResult = await vehiclesRepository.AddVehicleAsync(vehicle, cancellationToken);
            if (!createdResult.IsSuccess || createdResult.Value is null)
                return ApplicationResult<VehicleResponse>.Failure(
                    "Failed to create vehicle.", ResultError.Internal);

            var created = createdResult.Value;
            logger.LogInformation("Vehicle {VehicleId} ({RegistrationPlate}) created.", created.Id, created.RegistrationPlate);

            return ApplicationResult<VehicleResponse>.Success(new VehicleResponse(
                created.Id,
                created.ManufacturerId,
                created.Manufacturer?.Name ?? string.Empty,
                created.VehicleModel,
                created.AcrissCode,
                created.Category,
                created.Type,
                created.Transmission,
                created.Fuel,
                created.ManufacturingYear,
                created.KilometersDriven,
                created.EnginePowerInKw,
                created.RegistrationPlate,
                created.PricePerDayInEuro,
                created.Color));
        }
    }
}