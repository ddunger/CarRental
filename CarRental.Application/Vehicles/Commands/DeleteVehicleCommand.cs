using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Vehicles.Commands
{
    public record DeleteVehicleCommand(int VehicleId) : IRequest<ApplicationResult>;

    public class DeleteVehicleCommandHandler(
        ILogger<DeleteVehicleCommandHandler> logger,
        IVehiclesRepository vehiclesRepository)
        : IRequestHandler<DeleteVehicleCommand, ApplicationResult>
    {
        public async Task<ApplicationResult> Handle(
            DeleteVehicleCommand command, CancellationToken cancellationToken)
        {
            var deleteResult = await vehiclesRepository.DeleteVehicleAsync(command.VehicleId, cancellationToken);

            if (!deleteResult.IsSuccess)
                return ApplicationResult.Failure("Failed to delete vehicle.", ResultError.Internal);

            if (!deleteResult.Value)
                return ApplicationResult.Failure("Vehicle not found.", ResultError.NotFound);

            logger.LogInformation("Vehicle {VehicleId} deleted.", command.VehicleId);
            return ApplicationResult.Success();
        }
    }
}