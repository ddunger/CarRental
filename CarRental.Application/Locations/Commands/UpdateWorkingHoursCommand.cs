using CarRental.Application.Locations.Requests;
using CarRental.Application.Locations.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record UpdateWorkingHoursCommand(int LocationId, int HoursId, UpdateWorkingHoursRequest Request)
          : IRequest<ApplicationResult<WorkingHoursResponse>>;

    public class UpdateWorkingHoursCommandHandler(
        ILogger<UpdateWorkingHoursCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<UpdateWorkingHoursCommand, ApplicationResult<WorkingHoursResponse>>
    {
        public async Task<ApplicationResult<WorkingHoursResponse>> Handle(
            UpdateWorkingHoursCommand command, CancellationToken cancellationToken)
        {
            var hoursResult = await locationRepository.GetWorkingHoursByIdAsync(command.HoursId, cancellationToken);
            if (!hoursResult.IsSuccess)
                return ApplicationResult<WorkingHoursResponse>.Failure("Failed to retrieve working hours.", ResultError.Internal);
            if (hoursResult.Value is null)
                return ApplicationResult<WorkingHoursResponse>.Failure("Working hours not found.", ResultError.NotFound);
            if (hoursResult.Value.LocationId != command.LocationId)
                return ApplicationResult<WorkingHoursResponse>.Failure("Working hours do not belong to this location.", ResultError.Validation);

            var hours = hoursResult.Value;
            if (command.Request.OpenTime is not null) hours.OpenTime = command.Request.OpenTime.Value;
            if (command.Request.CloseTime is not null) hours.CloseTime = command.Request.CloseTime.Value;
            if (command.Request.IsClosed is not null) hours.IsClosed = command.Request.IsClosed.Value;

            var result = await locationRepository.UpdateWorkingHoursAsync(hours, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<WorkingHoursResponse>.Failure("Failed to update working hours.", ResultError.Internal);

            logger.LogInformation("Working hours {HoursId} updated for location {LocationId}.",
                command.HoursId, command.LocationId);
            return ApplicationResult<WorkingHoursResponse>.Success(new WorkingHoursResponse(
                hours.Id, hours.DayOfWeek, hours.OpenTime, hours.CloseTime, hours.IsClosed));
        }
    }
}
