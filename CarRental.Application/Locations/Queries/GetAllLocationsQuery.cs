using CarRental.Application.Locations.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Locations.Queries
{
    public record GetAllLocationsQuery(int? Offset, int? Limit)
        : IRequest<ApplicationResult<IEnumerable<LocationResponse>>>;

    public class GetAllLocationsQueryHandler(
        ILogger<GetAllLocationsQueryHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<GetAllLocationsQuery, ApplicationResult<IEnumerable<LocationResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<LocationResponse>>> Handle(
            GetAllLocationsQuery query, CancellationToken cancellationToken)
        {
            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<LocationResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);
            if (query.Limit.HasValue && query.Limit < 1)
                return ApplicationResult<IEnumerable<LocationResponse>>.Failure(
                    "Limit must be 1 or greater.", ResultError.Validation);

            var result = await locationRepository.GetAllAsync(query.Offset, query.Limit, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<IEnumerable<LocationResponse>>.Failure(
                    "Failed to retrieve locations.", ResultError.Internal);

            var locations = result.Value?.ToList() ?? [];
            logger.LogInformation("Fetched {Count} location(s).", locations.Count);

            return ApplicationResult<IEnumerable<LocationResponse>>.Success(
                locations.Select(l => new LocationResponse(
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
                        h.CloseTime)))));
        }
    }

}
