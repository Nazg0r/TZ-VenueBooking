using Microsoft.EntityFrameworkCore;

using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Repositories;

public class VenueRepository(VenueBookingContext context)
    : BaseRepository<Venue>(context), IVenueRepository
{
    public override async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Set
            .Include(v => v.Services)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public override async Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Set
            .AsNoTracking()
            .Include(v => v.Services)
            .ToListAsync(cancellationToken);
}