using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Errors;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Interfaces.Services;
using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.BusinessLogic.Services;

public sealed class VenueService(
    IVenueRepository venueRepository,
    IServiceRepository serviceRepository,
    IBookingRepository bookingRepository) : IVenueService
{
    public async Task<Result<IReadOnlyList<Venue>>> FindAvailableAsync(
        AvailableVenuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.EndUtc <= request.StartUtc) return BookingErrors.InvalidPeriod;

        var venues = await venueRepository.GetAllAsync(cancellationToken);
        var overlappingBookings =
            await bookingRepository.GetByPeriodAsync(request.StartUtc, request.EndUtc, cancellationToken);
        var bookedVenueIds = overlappingBookings.Select(booking => booking.VenueId).ToHashSet();

        IReadOnlyList<Venue> available = venues
            .Where(venue => venue.Capacity >= request.Capacity && !bookedVenueIds.Contains(venue.Id))
            .ToList();

        return Result<IReadOnlyList<Venue>>.Success(available);
    }

    public async Task<Result<Venue>> AddNewAsync(
        Venue newVenue,
        IReadOnlyList<Guid>? serviceIds,
        CancellationToken cancellationToken = default)
    {
        var servicesResult = await ResolveCatalogServicesAsync(serviceIds, cancellationToken);
        if (servicesResult.IsFailure) return servicesResult.Error;

        foreach (var service in servicesResult.Value) newVenue.Services.Add(service);

        await venueRepository.AddAsync(newVenue, cancellationToken);

        return newVenue;
    }

    public async Task<Result> UpdateAsync(Venue updatedVenue, CancellationToken cancellationToken = default)
    {
        var existing = await venueRepository.GetByIdAsync(updatedVenue.Id, cancellationToken);
        if (existing is null) return VenueErrors.NotFound(updatedVenue.Id);

        existing.Name = updatedVenue.Name;
        existing.Capacity = updatedVenue.Capacity;
        existing.BasePricePerHour = updatedVenue.BasePricePerHour;

        await venueRepository.UpdateAsync(existing, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await venueRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null) return VenueErrors.NotFound(id);

        await venueRepository.DeleteAsync(existing, cancellationToken);

        return Result.Success();
    }

    private async Task<Result<IReadOnlyList<Service>>> ResolveCatalogServicesAsync(
        IReadOnlyList<Guid>? serviceIds,
        CancellationToken cancellationToken)
    {
        if (serviceIds is null || serviceIds.Count == 0) return Result<IReadOnlyList<Service>>.Success([]);

        var catalog = await serviceRepository.GetByIdsAsync(serviceIds, cancellationToken);

        return RequestedServices.Match(serviceIds, catalog, VenueErrors.UnknownService);
    }
}