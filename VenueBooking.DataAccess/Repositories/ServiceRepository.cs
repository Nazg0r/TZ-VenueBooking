using Microsoft.EntityFrameworkCore;

using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Repositories;

public class ServiceRepository(VenueBookingContext context)
    : BaseRepository<Service>(context), IServiceRepository
{
    public async Task<IReadOnlyList<Service>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
        => await Set
            .Where(service => ids.Contains(service.Id))
            .ToListAsync(cancellationToken);
}