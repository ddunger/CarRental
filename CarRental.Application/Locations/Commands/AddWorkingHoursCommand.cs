using CarRental.Application.Locations.Requests;
using CarRental.Application.Locations.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record AddWorkingHoursCommand(int LocationId, AddWorkingHoursRequest Request)
         : IRequest<ApplicationResult<WorkingHoursResponse>>;

    public class AddWorkingHoursCommandHandler(
        ILogger<AddWorkingHoursCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<AddWorkingHoursCommand, ApplicationResult<WorkingHoursResponse>>
    {
        public async Task<ApplicationResult<WorkingHoursResponse>> Handle(
            AddWorkingHoursCommand command, CancellationToken cancellationToken)
        {
            var locationResult = await locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (!locationResult.IsSuccess)
                return ApplicationResult<WorkingHoursResponse>.Failure("Failed to retrieve location.", ResultError.Internal);
            if (locationResult.Value is null)
                return ApplicationResult<WorkingHoursResponse>.Failure("Location not found.", ResultError.NotFound);

            if (locationResult.Value.WorkingHours.Any(wh => wh.DayOfWeek == command.Request.DayOfWeek))
                return ApplicationResult<WorkingHoursResponse>.Failure(
                    $"Working hours for {command.Request.DayOfWeek} already exist for this location.", ResultError.Conflict);

            var workingHours = new LocationWorkingHoursEntity
            {
                LocationId = command.LocationId,
                DayOfWeek = command.Request.DayOfWeek,
                OpenTime = command.Request.OpenTime,
                CloseTime = command.Request.CloseTime,
                IsClosed = command.Request.IsClosed
            };

            var result = await locationRepository.AddWorkingHoursAsync(workingHours, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<WorkingHoursResponse>.Failure("Failed to add working hours.", ResultError.Internal);

            logger.LogInformation("Working hours added for location {LocationId} on {DayOfWeek}.",
                command.LocationId, command.Request.DayOfWeek);
            return ApplicationResult<WorkingHoursResponse>.Success(new WorkingHoursResponse(
                result.Value!.Id,
                result.Value.DayOfWeek,
                result.Value.OpenTime,
                result.Value.CloseTime,
                result.Value.IsClosed));
        }
    }
}
