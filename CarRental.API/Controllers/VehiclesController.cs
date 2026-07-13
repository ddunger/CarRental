using CarRental.API.Extensions;
using CarRental.Application.Vehicles.Commands;
using CarRental.Application.Vehicles.Queries;
using CarRental.Application.Vehicles.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VehiclesController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Returns a paginated list of all vehicles. Use <c>offset</c> and <c>limit</c> query parameters for pagination.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Customer")]

        public async Task<IActionResult> GetAllVehiclesAsync(
            [FromBody] GetVehiclesRequest request,
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllVehiclesQuery(request, offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single vehicle by their ID.
        /// </remarks>
        [HttpGet("{vehicleId:int}")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetVehicleByIdAsync([FromRoute] int vehicleId)
        {
            var result = await sender.Send(new GetVehicleByIdQuery(vehicleId));
            return result.ToActionResult(this);
        }


        /// <remarks>
        /// Creates a new vehicle. Returns 201 with the created resource location on success.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> CreateVehicleAsync(
            [FromBody] CreateVehicleRequest request)
        {
            var result = await sender.Send(new CreateVehicleCommand(request));

            if (result.IsSuccess)
                return Created($"api/vehicles/{result.Value!.Id}", new { data = result.Value });

            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates an existing vehicle.
        /// </remarks>
        [HttpPatch("{vehicleId:int}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> UpdateVehicleAsync(
            [FromRoute] int vehicleId,
            [FromBody] UpdateVehicleRequest request)
        {
            var result = await sender.Send(new UpdateVehicleCommand(vehicleId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Permanently deletes a vehicle by ID.
        /// </remarks>
        [HttpDelete("{vehicleId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleAsync(
            [FromRoute] int vehicleId)
        {
            var result = await sender.Send(new DeleteVehicleCommand(vehicleId));
            return result.ToActionResult(this);
        }

    }
}