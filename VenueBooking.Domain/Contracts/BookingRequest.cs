namespace VenueBooking.Domain.Contracts;

public sealed record BookingRequest(
    Guid VenueId,
    DateTime StartUtc,
    DateTime EndUtc,
    string CustomerName,
    IReadOnlyList<Guid>? ServiceIds);