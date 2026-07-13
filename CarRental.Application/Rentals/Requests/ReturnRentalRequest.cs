namespace CarRental.Application.Rentals.Requests
{
    public record ReturnRentalRequest(
        int? DropoffLocationId,                 // null = as agreed on the rental
        DateTimeOffset? ActualReturnTimeUtc);   // null = now
}