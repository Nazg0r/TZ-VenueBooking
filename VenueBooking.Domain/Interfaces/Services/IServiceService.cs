using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Interfaces.Services;

public interface IServiceService
{
    Task<Result<IReadOnlyList<Service>>> GetAllAsync(CancellationToken cancellationToken = default);
}
