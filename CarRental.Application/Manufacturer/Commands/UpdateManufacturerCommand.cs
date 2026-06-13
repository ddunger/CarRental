using CarRental.Application.Manufacturer.Responses;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Manufacturer.Commands
{

    public record UpdateManufacturerCommand(int ManufacturerId, string Name) : IRequest<ApplicationResult<ManufacturerResponse>>;

    public class UpdateManufacturerCommandHandler(
           ILogger<UpdateManufacturerCommandHandler> logger,
           IManufacturersRepository manufacturersRepository)
           : IRequestHandler<UpdateManufacturerCommand, ApplicationResult<ManufacturerResponse>>
    {
        public async Task<ApplicationResult<ManufacturerResponse>> Handle(
     UpdateManufacturerCommand command, CancellationToken cancellationToken)
        {
            var manufacturerResult = await manufacturersRepository.GetManufacturerByIdAsync(command.ManufacturerId, cancellationToken);
            if (!manufacturerResult.IsSuccess)
                return ApplicationResult<ManufacturerResponse>.Failure("Failed to retrieve manufacturer.", ResultError.Internal);
            if (manufacturerResult.Value is null)
                return ApplicationResult<ManufacturerResponse>.Failure($"Manufacturer {command.ManufacturerId} not found.", ResultError.NotFound);

            var manufacturer = manufacturerResult.Value;
            manufacturer.Name = command.Name;

            var updatedResult = await manufacturersRepository.UpdateManufacturerAsync(manufacturer, cancellationToken);
            if (!updatedResult.IsSuccess)
                return ApplicationResult<ManufacturerResponse>.Failure("Failed to update manufacturer.", ResultError.Internal);

            logger.LogInformation("Manufacturer {ManufacturerId} updated to name '{Name}'.", manufacturer.Id, manufacturer.Name);
            return ApplicationResult<ManufacturerResponse>.Success(new ManufacturerResponse(manufacturer.Id, manufacturer.Name));
        }
    }
}