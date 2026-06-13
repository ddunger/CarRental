using CarRental.API.Extensions;
using CarRental.Application.Manufacturer.Commands;
using CarRental.Application.Manufacturer.Queries;
using CarRental.Application.Manufacturer.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/manufacturer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ManufacturerController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Returns a paginated list of all manufacturers. Use <c>offset</c> and <c>limit</c> query parameters for pagination.
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllManufacturersAsync(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllManufacturersQuery(offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single manufacturer by their ID.
        /// </remarks>
        [HttpGet("{manufacturerId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetManufacturerByIdAsync([FromRoute] int manufacturerId)
        {
            var result = await sender.Send(new GetManufacturerByIdQuery(manufacturerId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Creates a new manufacturer. Returns 201 with the created resource location on success.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateManufacturerAsync(
            [FromBody] CreateManufacturerRequest request)
        {
            var result = await sender.Send(new CreateManufacturerCommand(request.Name));

            if (result.IsSuccess)
                return Created($"api/manufacturer/{result.Value!.Id}", new { data = result.Value });

            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates the name of an existing manufacturer.
        /// </remarks>
        [HttpPatch("{manufacturerId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateManufacturerAsync(
            [FromRoute] int manufacturerId,
            [FromBody] UpdateManufacturerRequest request)
        {
            var result = await sender.Send(new UpdateManufacturerCommand(manufacturerId, request.Name));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Permanently deletes a manufacturer by ID.
        /// </remarks>
        [HttpDelete("{manufacturerId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteManufacturerAsync(
            [FromRoute] int manufacturerId)
        {
            var result = await sender.Send(new DeleteManufacturerCommand(manufacturerId));
            return result.ToActionResult(this);
        }
    }
}