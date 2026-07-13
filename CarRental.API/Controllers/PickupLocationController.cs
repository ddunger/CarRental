using CarRental.API.Extensions;
using CarRental.Application.Locations.Commands;
using CarRental.Application.Locations.Queries;
using CarRental.Application.Locations.Requests;
using CarRental.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/location")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PickupLocationController(ISender sender) : ControllerBase
    {
        // --- Location ---

        /// <remarks>
        /// Returns a paginated list of all pickup locations including their working hours and holidays.
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllLocationsAsync(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllLocationsQuery(offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single pickup location by ID, including working hours and holidays.
        /// </remarks>
        [HttpGet("{locationId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocationByIdAsync([FromRoute] int locationId)
        {
            var result = await sender.Send(new GetLocationByIdQuery(locationId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Creates a new pickup location. Admin and Manager access only.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> CreateLocationAsync([FromBody] CreateLocationRequest request)
        {
            var result = await sender.Send(new CreateLocationCommand(request));
            if (result.IsSuccess)
                return Created($"api/location/{result.Value!.Id}", new { data = result.Value });
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates an existing pickup location's details. Admin and Manager access only.
        /// </remarks>
        [HttpPatch("{locationId:int}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> UpdateLocationAsync(
            [FromRoute] int locationId,
            [FromBody] UpdateLocationRequest request)
        {
            var result = await sender.Send(new UpdateLocationCommand(locationId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Permanently deletes a pickup location. Admin access only.
        /// </remarks>
        [HttpDelete("{locationId:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteLocationAsync([FromRoute] int locationId)
        {
            var result = await sender.Send(new DeleteLocationCommand(locationId));
            return result.ToActionResult(this);
        }

        // --- Working Hours ---

        /// <remarks>
        /// Adds working hours for a specific day at a pickup location. Admin and Manager access only.
        /// </remarks>
        [HttpPost("{locationId:int}/hours")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> AddWorkingHoursAsync(
            [FromRoute] int locationId,
            [FromBody] AddWorkingHoursRequest request)
        {
            var result = await sender.Send(new AddWorkingHoursCommand(locationId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates working hours for a specific day at a pickup location. Admin and Manager access only.
        /// </remarks>
        [HttpPatch("{locationId:int}/hours/{hoursId:int}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> UpdateWorkingHoursAsync(
            [FromRoute] int locationId,
            [FromRoute] int hoursId,
            [FromBody] UpdateWorkingHoursRequest request)
        {
            var result = await sender.Send(new UpdateWorkingHoursCommand(locationId, hoursId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Deletes working hours entry for a specific day at a pickup location. Admin access only.
        /// </remarks>
        [HttpDelete("{locationId:int}/hours/{hoursId:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteWorkingHoursAsync(
            [FromRoute] int locationId,
            [FromRoute] int hoursId)
        {
            var result = await sender.Send(new DeleteWorkingHoursCommand(locationId, hoursId));
            return result.ToActionResult(this);
        }

        // --- Holidays ---

        /// <remarks>
        /// Adds a holiday entry for a pickup location, optionally with modified opening hours. Admin and Manager access only.
        /// </remarks>
        [HttpPost("{locationId:int}/holiday")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> AddHolidayAsync(
            [FromRoute] int locationId,
            [FromBody] AddHolidayRequest request)
        {
            var result = await sender.Send(new AddHolidayCommand(locationId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Updates a holiday entry for a pickup location. Admin and Manager access only.
        /// </remarks>
        [HttpPatch("{locationId:int}/holiday/{holidayId:int}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
        public async Task<IActionResult> UpdateHolidayAsync(
            [FromRoute] int locationId,
            [FromRoute] int holidayId,
            [FromBody] UpdateHolidayRequest request)
        {
            var result = await sender.Send(new UpdateHolidayCommand(locationId, holidayId, request));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Deletes a holiday entry for a pickup location. Admin access only.
        /// </remarks>
        [HttpDelete("{locationId:int}/holiday/{holidayId:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteHolidayAsync(
            [FromRoute] int locationId,
            [FromRoute] int holidayId)
        {
            var result = await sender.Send(new DeleteHolidayCommand(locationId, holidayId));
            return result.ToActionResult(this);
        }
    }
}