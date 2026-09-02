using Microsoft.EntityFrameworkCore;

using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Enums;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Repositories;

public class BookingRepository(VenueBookingContext context)
    : BaseRepository<Booking>(context), IBookingRepository
{
    // Отримання бронювань за період часу
    public async Task<IReadOnlyList<Booking>> GetByPeriodAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
        => await Set
            .AsNoTracking()
            .Where(booking => booking.Status != BookingStatus.Cancelled
                              && booking.StartUtc < toUtc
                              && booking.EndUtc > fromUtc)
            .OrderBy(booking => booking.StartUtc)
            .ToListAsync(cancellationToken);

    // Перевірка наявності перекриття бронювань для конкретного залу
    public Task<bool> HasOverlapAsync(
        Guid venueId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken cancellationToken = default)
        => Set
            .AsNoTracking()
            .AnyAsync(booking => booking.VenueId == venueId
                                 && booking.Status != BookingStatus.Cancelled
                                 && booking.StartUtc < endUtc
                                 && booking.EndUtc > startUtc, cancellationToken);

    // Отримання бронювань для звітів з фільтрацією за залом та періодом часу
    public async Task<IReadOnlyList<Booking>> GetForReportsAsync(
        Guid? venueId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = Set
            .AsNoTracking()
            .Where(booking => booking.Status != BookingStatus.Cancelled);

        if (venueId is { } id) query = query.Where(booking => booking.VenueId == id);
        if (fromUtc is { } from) query = query.Where(booking => booking.StartUtc >= from);
        if (toUtc is { } to) query = query.Where(booking => booking.StartUtc < to);

        return await query
            .OrderBy(booking => booking.StartUtc)
            .ToListAsync(cancellationToken);
    }
}