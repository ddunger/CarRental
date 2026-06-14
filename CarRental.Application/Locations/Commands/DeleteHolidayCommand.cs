using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record DeleteHolidayCommand(int LocationId, int HolidayId)
       : IRequest<ApplicationResult>;

    public class DeleteHolidayCommandHandler(
        ILogger<DeleteHolidayCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<DeleteHolidayCommand, ApplicationResult>
    {
        public async Task<ApplicationResult> Handle(
            DeleteHolidayCommand command, CancellationToken cancellationToken)
        {
            var holidayResult = await locationRepository.GetHolidayByIdAsync(command.HolidayId, cancellationToken);
            if (!holidayResult.IsSuccess)
                return ApplicationResult.Failure("Failed to retrieve holiday.", ResultError.Internal);
            if (holidayResult.Value is null)
                return ApplicationResult.Failure("Holiday not found.", ResultError.NotFound);
            if (holidayResult.Value.LocationId != command.LocationId)
                return ApplicationResult.Failure("Holiday does not belong to this location.", ResultError.Validation);

            var result = await locationRepository.DeleteHolidayAsync(command.HolidayId, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult.Failure("Failed to delete holiday.", ResultError.Internal);

            logger.LogInformation("Holiday {HolidayId} deleted from location {LocationId}.",
                command.HolidayId, command.LocationId);
            return ApplicationResult.Success();
        }
    }
}