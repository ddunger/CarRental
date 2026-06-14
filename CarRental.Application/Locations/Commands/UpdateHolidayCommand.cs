using CarRental.Application.Locations.Requests;
using CarRental.Application.Locations.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Commands
{
    public record UpdateHolidayCommand(int LocationId, int HolidayId, UpdateHolidayRequest Request)
            : IRequest<ApplicationResult<HolidayResponse>>;

    public class UpdateHolidayCommandHandler(
        ILogger<UpdateHolidayCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<UpdateHolidayCommand, ApplicationResult<HolidayResponse>>
    {
        public async Task<ApplicationResult<HolidayResponse>> Handle(
            UpdateHolidayCommand command, CancellationToken cancellationToken)
        {
            var holidayResult = await locationRepository.GetHolidayByIdAsync(command.HolidayId, cancellationToken);
            if (!holidayResult.IsSuccess)
                return ApplicationResult<HolidayResponse>.Failure("Failed to retrieve holiday.", ResultError.Internal);
            if (holidayResult.Value is null)
                return ApplicationResult<HolidayResponse>.Failure("Holiday not found.", ResultError.NotFound);
            if (holidayResult.Value.LocationId != command.LocationId)
                return ApplicationResult<HolidayResponse>.Failure("Holiday does not belong to this location.", ResultError.Validation);

            var holiday = holidayResult.Value;
            if (command.Request.HolidayName is not null) holiday.HolidayName = command.Request.HolidayName;
            if (command.Request.IsClosed is not null) holiday.IsClosed = command.Request.IsClosed.Value;
            if (command.Request.OpenTime is not null) holiday.OpenTime = command.Request.OpenTime;
            if (command.Request.CloseTime is not null) holiday.CloseTime = command.Request.CloseTime;

            var result = await locationRepository.UpdateHolidayAsync(holiday, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<HolidayResponse>.Failure("Failed to update holiday.", ResultError.Internal);

            logger.LogInformation("Holiday {HolidayId} updated for location {LocationId}.",
                command.HolidayId, command.LocationId);
            return ApplicationResult<HolidayResponse>.Success(new HolidayResponse(
                holiday.Id,
                holiday.Date,
                holiday.HolidayName,
                holiday.IsClosed,
                holiday.OpenTime,
                holiday.CloseTime));
        }
    }
}
