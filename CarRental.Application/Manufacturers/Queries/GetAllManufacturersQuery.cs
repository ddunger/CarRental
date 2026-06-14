using CarRental.Application.Manufacturers.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Manufacturers.Queries
{
    public record GetAllManufacturersQuery(int? Offset, int? Limit) : IRequest<ApplicationResult<IEnumerable<ManufacturerResponse>>>;


    public class GetAllManufacturersQueryHandler(
        ILogger<GetAllManufacturersQueryHandler> logger,
        IManufacturersRepository manufacturersRepository)
        : IRequestHandler<GetAllManufacturersQuery, ApplicationResult<IEnumerable<ManufacturerResponse>>>
    {
        public async Task<ApplicationResult<IEnumerable<ManufacturerResponse>>> Handle(
            GetAllManufacturersQuery query, CancellationToken cancellationToken)
        {
            if (query.Offset.HasValue && query.Offset < 0)
                return ApplicationResult<IEnumerable<ManufacturerResponse>>.Failure(
                    "Offset must be zero or greater.", ResultError.Validation);

            if (query.Limit.HasValue && (query.Limit < 1))
                return ApplicationResult<IEnumerable<ManufacturerResponse>>.Failure(
                    "Limit must be 1 or greater", ResultError.Validation);

            var manufacturersResult = await manufacturersRepository.GetAllManufacturersAsync(
                query.Offset, query.Limit, cancellationToken);

            if (!manufacturersResult.IsSuccess)
            {
                return ApplicationResult<IEnumerable<ManufacturerResponse>>.Failure(
                    "Failed to retrieve manufacturers.",
                    ResultError.Internal);
            }

            var manufacturers = (manufacturersResult.Value ?? Enumerable.Empty<ManufacturerEntity>()).ToList();
            logger.LogInformation("Fetched {Count} manufacturer(s)", manufacturers.Count);

            return ApplicationResult<IEnumerable<ManufacturerResponse>>.Success(
                manufacturers.Select(m => new ManufacturerResponse(m.Id, m.Name)));
        }
    }
}
