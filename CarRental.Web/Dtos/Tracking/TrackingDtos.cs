using CarRental.Web.Dtos.Rentals;
using CarRental.Web.Dtos.Reservations;

namespace CarRental.Web.Dtos.Tracking
{
    public record TrackingResponse(
        ReservationResponse? Reservation,
        RentalResponse? Rental);
}
