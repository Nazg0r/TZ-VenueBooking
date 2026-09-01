using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Interfaces.Services;

public interface IBookingService
{
    Task<Result<BookingConfirmation>> BookAsync(
        BookingRequest request,
        CancellationToken cancellationToken = default);
}