using CarRental.Application.Locations.Requests;
using CarRental.Application.Locations.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record UpdateLocationCommand(int LocationId, UpdateLocationRequest Request)
         : IRequest<ApplicationResult<LocationResponse>>;

    public class UpdateLocationCommandHandler(
        ILogger<UpdateLocationCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<UpdateLocationCommand, ApplicationResult<LocationResponse>>
    {
        public async Task<ApplicationResult<LocationResponse>> Handle(
            UpdateLocationCommand command, CancellationToken cancellationToken)
        {
            var locationResult = await locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (!locationResult.IsSuccess)
                return ApplicationResult<LocationResponse>.Failure("Failed to retrieve location.", ResultError.Internal);
            if (locationResult.Value is null)
                return ApplicationResult<LocationResponse>.Failure("Location not found.", ResultError.NotFound);

            var location = locationResult.Value;
            if (command.Request.Name is not null) location.Name = command.Request.Name;
            if (command.Request.Address is not null) location.Address = command.Request.Address;
            if (command.Request.Latitude is not null) location.Latitude = command.Request.Latitude.Value;
            if (command.Request.Longitude is not null) location.Longitude = command.Request.Longitude.Value;
            if (command.Request.IsActive is not null) location.IsActive = command.Request.IsActive.Value;

            var result = await locationRepository.UpdateAsync(location, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<LocationResponse>.Failure("Failed to update location.", ResultError.Internal);

            logger.LogInformation("Location {LocationId} updated.", location.Id);
            return ApplicationResult<LocationResponse>.Success(new LocationResponse(
                location.Id,
                location.Name,
                location.Address,
                location.Latitude,
                location.Longitude,
                location.IsActive,
                location.WorkingHours.Select(wh => new WorkingHoursResponse(
                    wh.Id, wh.DayOfWeek, wh.OpenTime, wh.CloseTime, wh.IsClosed)),
                location.Holidays.Select(h => new HolidayResponse(
                    h.Id, h.Date, h.HolidayName, h.IsClosed, h.OpenTime, h.CloseTime))));
        }
    }
}
