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
    public record AddHolidayCommand(int LocationId, AddHolidayRequest Request)
        : IRequest<ApplicationResult<HolidayResponse>>;

    public class AddHolidayCommandHandler(
        ILogger<AddHolidayCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<AddHolidayCommand, ApplicationResult<HolidayResponse>>
    {
        public async Task<ApplicationResult<HolidayResponse>> Handle(
            AddHolidayCommand command, CancellationToken cancellationToken)
        {
            var locationResult = await locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (!locationResult.IsSuccess)
                return ApplicationResult<HolidayResponse>.Failure("Failed to retrieve location.", ResultError.Internal);
            if (locationResult.Value is null)
                return ApplicationResult<HolidayResponse>.Failure("Location not found.", ResultError.NotFound);

            if (locationResult.Value.Holidays.Any(h => h.Date == command.Request.Date))
                return ApplicationResult<HolidayResponse>.Failure(
                    $"A holiday entry for {command.Request.Date} already exists for this location.", ResultError.Conflict);

            var holiday = new LocationHolidayEntity
            {
                LocationId = command.LocationId,
                Date = command.Request.Date,
                HolidayName = command.Request.HolidayName,
                IsClosed = command.Request.IsClosed,
                OpenTime = command.Request.OpenTime,
                CloseTime = command.Request.CloseTime
            };

            var result = await locationRepository.AddHolidayAsync(holiday, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<HolidayResponse>.Failure("Failed to add holiday.", ResultError.Internal);

            logger.LogInformation("Holiday added for location {LocationId} on {Date}.",
                command.LocationId, command.Request.Date);
            return ApplicationResult<HolidayResponse>.Success(new HolidayResponse(
                result.Value!.Id,
                result.Value.Date,
                result.Value.HolidayName,
                result.Value.IsClosed,
                result.Value.OpenTime,
                result.Value.CloseTime));
        }
    }
}
