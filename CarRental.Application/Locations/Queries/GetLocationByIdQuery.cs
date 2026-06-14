using CarRental.Application.Locations.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Queries
{
    public record GetLocationByIdQuery(int LocationId)
          : IRequest<ApplicationResult<LocationResponse>>;

    public class GetLocationByIdQueryHandler(
        ILogger<GetLocationByIdQueryHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<GetLocationByIdQuery, ApplicationResult<LocationResponse>>
    {
        public async Task<ApplicationResult<LocationResponse>> Handle(
            GetLocationByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await locationRepository.GetByIdAsync(query.LocationId, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<LocationResponse>.Failure(
                    "Failed to retrieve location.", ResultError.Internal);
            if (result.Value is null)
            {
                logger.LogWarning("Location {LocationId} not found.", query.LocationId);
                return ApplicationResult<LocationResponse>.Failure(
                    "Location not found.", ResultError.NotFound);
            }

            var l = result.Value;
            return ApplicationResult<LocationResponse>.Success(new LocationResponse(
                l.Id,
                l.Name,
                l.Address,
                l.Latitude,
                l.Longitude,
                l.IsActive,
                l.WorkingHours.Select(wh => new WorkingHoursResponse(
                    wh.Id,
                    wh.DayOfWeek,
                    wh.OpenTime,
                    wh.CloseTime,
                    wh.IsClosed)),
                l.Holidays.Select(h => new HolidayResponse(
                    h.Id,
                    h.Date,
                    h.HolidayName,
                    h.IsClosed,
                    h.OpenTime,
                    h.CloseTime))));
        }
    }
}

