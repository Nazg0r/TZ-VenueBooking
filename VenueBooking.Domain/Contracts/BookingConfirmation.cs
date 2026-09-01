namespace VenueBooking.Domain.Contracts;

public sealed record BookingConfirmation(
    Guid BookingId,
    decimal RentalCost,
    decimal ServicesCost,
    decimal TotalCost);