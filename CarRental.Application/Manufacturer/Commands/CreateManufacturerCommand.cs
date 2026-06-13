using CarRental.Application.Manufacturer.Responses;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Domain.Interfaces.Repositories;
using CarRental.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Manufacturer.Commands
{
    public record CreateManufacturerCommand(string Name) : IRequest<ApplicationResult<ManufacturerResponse>>;

    public class CreateManufacturerCommandHandler(
        ILogger<CreateManufacturerCommandHandler> logger,
        IManufacturersRepository manufacturersRepository)
        : IRequestHandler<CreateManufacturerCommand, ApplicationResult<ManufacturerResponse>>
    {
        public async Task<ApplicationResult<ManufacturerResponse>> Handle(CreateManufacturerCommand command, CancellationToken cancellationToken)
        {
            var manufacturer = new ManufacturerEntity { Name = command.Name };
            var createdResult = await manufacturersRepository.AddManufacturerAsync(manufacturer, cancellationToken);
            if (!createdResult.IsSuccess || createdResult.Value == null)
                return ApplicationResult<ManufacturerResponse>.Failure("Failed to create manufacturer.", ResultError.Internal);
            var created = createdResult.Value;
            logger.LogInformation("Manufacturer {ManufacturerId} created.", created.Id);
            return ApplicationResult<ManufacturerResponse>.Success(new ManufacturerResponse(created.Id, created.Name));
        }
    }
}