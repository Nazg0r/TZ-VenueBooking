using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Errors;

public static class VenueErrors
{
    public static Error NotFound(Guid venueId) => Error.NotFound(
        "Venues.NotFound", $"The venue with Id '{venueId}' was not found.");

    public static Error UnknownService(Guid serviceId) => Error.Validation(
        "Venues.UnknownService", $"The service with Id '{serviceId}' does not exist in the catalog.");
}