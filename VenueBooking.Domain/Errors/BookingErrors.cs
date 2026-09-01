using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Errors;

public static class BookingErrors
{
    public static readonly Error InvalidPeriod = Error.Validation(
        "Bookings.InvalidPeriod", "The booking end time must be after the start time.");

    public static readonly Error PeriodInThePast = Error.Validation(
        "Bookings.PeriodInThePast", "The booking cannot start in the past.");

    public static readonly Error PeriodSpansMultipleDays = Error.Validation(
        "Bookings.PeriodSpansMultipleDays", "The booking must start and end on the same day.");

    public static readonly Error VenueAlreadyBooked = Error.Conflict(
        "Bookings.VenueAlreadyBooked", "The venue is already booked for the specified period.");

    public static Error ServiceNotOfferedByVenue(Guid serviceId) => Error.Validation(
        "Bookings.ServiceNotOffered", $"The service '{serviceId}' is not offered by this venue.");
}