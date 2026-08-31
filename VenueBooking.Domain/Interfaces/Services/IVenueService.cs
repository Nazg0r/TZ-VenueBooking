using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Interfaces.Services;

public interface IVenueService
{
    Task<Result<IReadOnlyList<Venue>>> FindAvailableAsync(
        AvailableVenuesRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<Venue>> AddNewAsync(
        Venue venue,
        IReadOnlyList<Guid>? serviceIds,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Venue updatedVenue, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}