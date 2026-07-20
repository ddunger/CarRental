using CarRental.Application.Rentals.Responses;
using CarRental.Application.Reservations.Responses;

namespace CarRental.Application.Tracking.Responses
{
    public record TrackingResponse(
         ReservationResponse? Reservation,
         RentalResponse? Rental);
}
