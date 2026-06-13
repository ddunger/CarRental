using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Manufacturer.Commands
{
    public record DeleteManufacturerCommand(int ManufacturerId) : IRequest<ApplicationResult>;


    public class DeleteManufacturerCommandHandler(
        ILogger<DeleteManufacturerCommandHandler> logger,
        IManufacturersRepository manufacturersRepository)
        : IRequestHandler<DeleteManufacturerCommand, ApplicationResult>
    {
        public async Task<ApplicationResult> Handle(
        DeleteManufacturerCommand command, CancellationToken cancellationToken)
        {
            var manufacturerResult = await manufacturersRepository.GetManufacturerByIdAsync(command.ManufacturerId, cancellationToken);
            if (!manufacturerResult.IsSuccess)
                return ApplicationResult.Failure("Failed to retrieve manufacturer.", ResultError.Internal);
            if (manufacturerResult.Value is null)
                return ApplicationResult.Failure("Manufacturer not found.", ResultError.NotFound);

            var deletedResult = await manufacturersRepository.DeleteManufacturerAsync(command.ManufacturerId, cancellationToken);
            if (!deletedResult.IsSuccess)
                return ApplicationResult.Failure("Failed to delete manufacturer.", ResultError.Internal);

            logger.LogInformation("Manufacturer {ManufacturerId} deleted.", command.ManufacturerId);
            return ApplicationResult.Success();
        }
    }
}




