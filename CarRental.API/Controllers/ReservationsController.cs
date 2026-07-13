using CarRental.API.Extensions;
using CarRental.Application.Reservations.Commands;
using CarRental.Application.Reservations.Queries;
using CarRental.Application.Reservations.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReservationsController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Returns a paginated list of reservations.
        /// Admins and Managers see all reservations and can filter by customer;
        /// Customers only ever see their own (the <c>customerId</c> filter is ignored for them).
        /// Optional filters: vehicle, pickup location, status, pickup time range.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetAllReservationsAsync(
            [FromQuery] GetReservationsRequest request,
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetAllReservationsQuery(request, offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns a single reservation by ID.
        /// Customers can only retrieve their own reservations; Admins and Managers can retrieve any.
        /// </remarks>
        [HttpGet("{reservationId:int}")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetReservationByIdAsync([FromRoute] int reservationId)
        {
            var result = await sender.Send(new GetReservationByIdQuery(reservationId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Returns the authenticated user's own reservations, regardless of role.
        /// </remarks>
        [HttpGet("my")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> GetMyReservationsAsync(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var result = await sender.Send(new GetMyReservationsQuery(offset, limit));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Creates a new reservation. The vehicle must be available for the requested period,
        /// and pickup/return times must fall within the locations' working hours.
        /// Customers always book for themselves; Admins and Managers can book on behalf of any customer
        /// by supplying <c>CustomerId</c> in the request (ignored for Customers).
        /// Expected cost is calculated server-side from the vehicle's daily price.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationRequest request)
        {
            var result = await sender.Send(new CreateReservationCommand(request));

            if (result.IsSuccess)
                return Created($"api/reservations/{result.Value!.Id}", new { data = result.Value });

            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Confirms a pending reservation. Admin and Manager access only.
        /// Only reservations in <c>Pending</c> status can be confirmed.
        /// </remarks>
        [HttpPatch("{reservationId:int}/confirm")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ConfirmReservationAsync([FromRoute] int reservationId)
        {
            var result = await sender.Send(new ConfirmReservationCommand(reservationId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Cancels a reservation. Customers can cancel their own reservations;
        /// Admins and Managers can cancel any. Only <c>Pending</c> or <c>Confirmed</c>
        /// reservations can be cancelled.
        /// </remarks>
        [HttpPatch("{reservationId:int}/cancel")]
        [Authorize(Roles = "Admin, Manager, Customer")]
        public async Task<IActionResult> CancelReservationAsync([FromRoute] int reservationId)
        {
            var result = await sender.Send(new CancelReservationCommand(reservationId));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Marks a confirmed reservation as a no-show after the customer failed to pick up
        /// the vehicle. Admin and Manager access only.
        /// </remarks>
        [HttpPatch("{reservationId:int}/no-show")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> MarkNoShowAsync([FromRoute] int reservationId)
        {
            var result = await sender.Send(new MarkReservationNoShowCommand(reservationId));
            return result.ToActionResult(this);
        }
    }
}