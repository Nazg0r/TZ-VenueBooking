namespace VenueBooking.Domain.Contracts;

public sealed record AvailableVenuesRequest(
    DateTime StartUtc,
    DateTime EndUtc,
    int Capacity);