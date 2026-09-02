using VenueBooking.Domain.Models;

namespace VenueBooking.Domain.Interfaces.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IReadOnlyList<Booking>> GetByPeriodAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        Guid venueId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken = default);

    // Активні бронювання для звітів: опційно по залу та по межах періоду (за StartUtc).
    Task<IReadOnlyList<Booking>> GetForReportsAsync(
        Guid? venueId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);
}
