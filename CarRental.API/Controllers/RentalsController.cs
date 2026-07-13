using CarRental.API.Extensions;
using CarRental.Application.Rentals.Commands;
using CarRental.Application.Rentals.Queries;
using CarRental.Application.Rentals.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/rentals")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentalsController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Returns a paginated list of rentals.
        /// Admins and Managers see all rentals and can filter by customer;
        /// Customers only ever see their own (the <c>customerId</c> filter is ignored for them).
        /// Optional filters: vehicle, pickup location, status, pickup time range.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetAllRentalsAsync(
            [FromQuery] GetRentalsRequest request,
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllRentalsQuery(request, offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns the authenticated user's own rentals, regardless of role.
        /// </remarks>
        [HttpGet("my")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetMyRentalsAsync(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetMyRentalsQuery(offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single rental by ID.
        /// Customers can only retrieve their own rentals; Admins and Managers can retrieve any.
        /// </remarks>
        [HttpGet("{rentalId:int}")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetRentalByIdAsync([FromRoute] int rentalId)
        {
            var result = await sender.Send(new GetRentalByIdQuery(rentalId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Creates a rental (vehicle handover). Admin and Manager access only.
        ///
        /// Two modes:
        /// - **From a reservation:** supply only <c>ReservationId</c> — customer, vehicle,
        ///   locations, return time, and cost are taken from the confirmed reservation,
        ///   which is then marked as <c>Converted</c>.
        /// - **Walk-in:** leave <c>ReservationId</c> null and supply <c>CustomerId</c>,
        ///   <c>VehicleId</c>, <c>PickupLocationId</c>, <c>DropoffLocationId</c> and
        ///   <c>ExpectedReturnTimeUtc</c>. Cost is calculated from the vehicle's daily price.
        ///
        /// The vehicle must not be currently rented out, and for walk-ins must not collide
        /// with upcoming reservations.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> CreateRentalAsync([FromBody] CreateRentalRequest request)
        {
            var result = await sender.Send(new CreateRentalCommand(request));

            if (result.IsSuccess)
                return Created($"api/rentals/{result.Value!.Id}", new { data = result.Value });

            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Completes a rental (vehicle return). Admin and Manager access only.
        /// Optionally accepts a different dropoff location and an explicit return time
        /// (defaults to now). The final cost is recalculated from the actual duration —
        /// late returns are charged extra started days; early returns are not refunded.
        /// </remarks>
        [HttpPatch("{rentalId:int}/return")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ReturnRentalAsync(
            [FromRoute] int rentalId,
            [FromBody] ReturnRentalRequest request)
        {
            var result = await sender.Send(new ReturnRentalCommand(rentalId, request));
            return result.ToActionResult(this);
        }
    }
}