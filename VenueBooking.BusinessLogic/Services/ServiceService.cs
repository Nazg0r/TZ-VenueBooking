using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Interfaces.Services;
using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.BusinessLogic.Services;

public sealed class ServiceService(IServiceRepository serviceRepository) : IServiceService
{
    public async Task<Result<IReadOnlyList<Service>>> GetAllAsync(CancellationToken cancellationToken = default)
        => Result<IReadOnlyList<Service>>.Success(await serviceRepository.GetAllAsync(cancellationToken));
}
