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
    public record CreateLocationCommand(CreateLocationRequest Request)
         : IRequest<ApplicationResult<LocationResponse>>;

    public class CreateLocationCommandHandler(
        ILogger<CreateLocationCommandHandler> logger,
        IPickupLocationRepository locationRepository)
        : IRequestHandler<CreateLocationCommand, ApplicationResult<LocationResponse>>
    {
        public async Task<ApplicationResult<LocationResponse>> Handle(
            CreateLocationCommand command, CancellationToken cancellationToken)
        {
            var location = new PickupLocationEntity
            {
                Name = command.Request.Name,
                Address = command.Request.Address,
                Latitude = command.Request.Latitude,
                Longitude = command.Request.Longitude
            };

            var result = await locationRepository.AddAsync(location, cancellationToken);
            if (!result.IsSuccess)
                return ApplicationResult<LocationResponse>.Failure("Failed to create location.", ResultError.Internal);

            logger.LogInformation("Location {LocationId} created.", result.Value!.Id);
            return ApplicationResult<LocationResponse>.Success(new LocationResponse(
                result.Value.Id,
                result.Value.Name,
                result.Value.Address,
                result.Value.Latitude,
                result.Value.Longitude,
                result.Value.IsActive,
                [],
                []));
        }
    }
}
