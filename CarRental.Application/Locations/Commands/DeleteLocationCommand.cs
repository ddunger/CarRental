using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record DeleteLocationCommand(int LocationId)
         : IRequest<ApplicationResult>;

    public class DeleteLocationCommandHandler(
        ILogger<DeleteLocationCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<DeleteLocationCommand, ApplicationResult>
    {
        public async Task<ApplicationResult> Handle(
            DeleteLocationCommand command, CancellationToken cancellationToken)
        {
            var locationResult = await locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (!locationResult.IsSuccess)
                return ApplicationResult.Failure("Failed to retrieve location.", ResultError.Internal);
            if (locationResult.Value is null)
                return ApplicationResult.Failure("Location not found.", ResultError.NotFound);

            var result = await locationRepository.DeleteAsync(command.LocationId, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult.Failure("Failed to delete location.", ResultError.Internal);

            logger.LogInformation("Location {LocationId} deleted.", command.LocationId);
            return ApplicationResult.Success();
        }
    }

}
