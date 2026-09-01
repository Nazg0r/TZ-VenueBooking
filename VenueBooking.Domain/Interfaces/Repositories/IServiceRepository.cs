using VenueBooking.Domain.Models;

namespace VenueBooking.Domain.Interfaces.Repositories;

public interface IServiceRepository : IRepository<Service>
{
    Task<IReadOnlyList<Service>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default);
}