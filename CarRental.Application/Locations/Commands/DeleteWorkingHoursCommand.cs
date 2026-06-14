using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record DeleteWorkingHoursCommand(int LocationId, int HoursId)
        : IRequest<ApplicationResult>;

    public class DeleteWorkingHoursCommandHandler(
        ILogger<DeleteWorkingHoursCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<DeleteWorkingHoursCommand, ApplicationResult>
    {
        public async Task<ApplicationResult> Handle(
            DeleteWorkingHoursCommand command, CancellationToken cancellationToken)
        {
            var hoursResult = await locationRepository.GetWorkingHoursByIdAsync(command.HoursId, cancellationToken);
            if (!hoursResult.IsSuccess)
                return ApplicationResult.Failure("Failed to retrieve working hours.", ResultError.Internal);
            if (hoursResult.Value is null)
                return ApplicationResult.Failure("Working hours not found.", ResultError.NotFound);
            if (hoursResult.Value.LocationId != command.LocationId)
                return ApplicationResult.Failure("Working hours do not belong to this location.", ResultError.Validation);

            var result = await locationRepository.DeleteWorkingHoursAsync(command.HoursId, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult.Failure("Failed to delete working hours.", ResultError.Internal);

            logger.LogInformation("Working hours {HoursId} deleted from location {LocationId}.",
                command.HoursId, command.LocationId);
            return ApplicationResult.Success();
        }
    }
}
