using CarRental.API.Extensions;
using CarRental.Application.Tracking.Commands;
using CarRental.Application.Tracking.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [ApiController]
    [Route("api/tracking")]
    public class TrackingController(ISender sender) : ControllerBase
    {
        /// <remarks>
        /// Anonymous tracking of a reservation or rental by its tracking code.
        /// A reservation code also returns the linked rental once the reservation is converted;
        /// a walk-in rental code returns the rental only.
        /// </remarks>
        [HttpGet("{trackingCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrackingInfoAsync([FromRoute] string trackingCode)
        {
            var result = await sender.Send(new GetTrackingInfoQuery(trackingCode));
            return result.ToActionResult(this);
        }

        /// <remarks>
        /// Cancels a pending or confirmed reservation using its tracking code.
        /// Possession of the code acts as authorization.
        /// </remarks>
        [HttpPatch("{trackingCode}/cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelAsync([FromRoute] string trackingCode)
        {
            var result = await sender.Send(new CancelByTrackingCodeCommand(trackingCode));
            return result.ToActionResult(this);
        }
    }
}