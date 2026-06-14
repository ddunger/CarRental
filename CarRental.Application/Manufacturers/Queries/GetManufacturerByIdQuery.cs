using CarRental.Application.Manufacturers.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Manufacturers.Queries
{
    public record GetManufacturerByIdQuery(int ManufacturerId) 
    : IRequest<ApplicationResult<ManufacturerResponse>>;


    public class GetManufacturerByIdQueryHandler(
    ILogger<GetManufacturerByIdQueryHandler> logger,
    IManufacturersRepository manufacturersRepository)
    : IRequestHandler<GetManufacturerByIdQuery, ApplicationResult<ManufacturerResponse>>
    {
        public async Task<ApplicationResult<ManufacturerResponse>> Handle(
            GetManufacturerByIdQuery query, CancellationToken cancellationToken)
        {
            var manufacturerResult = await manufacturersRepository
                .GetManufacturerByIdAsync(query.ManufacturerId, cancellationToken);

            if (!manufacturerResult.IsSuccess)
                return ApplicationResult<ManufacturerResponse>.Failure("Failed to retrieve manufacturer.", ResultError.Internal);

            if (manufacturerResult.Value is null)
            {
                logger.LogWarning("Manufacturer {ManufacturerId} not found.", query.ManufacturerId);
                return ApplicationResult<ManufacturerResponse>.Failure("Manufacturer not found.", ResultError.NotFound);
            }

            var manufacturer = manufacturerResult.Value;
            return ApplicationResult<ManufacturerResponse>.Success(new ManufacturerResponse(manufacturer.Id, manufacturer.Name));
        }
    }
}




