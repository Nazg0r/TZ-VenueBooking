using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Enums;
using VenueBooking.Domain.Errors;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Interfaces.Services;
using VenueBooking.Domain.Models;
using VenueBooking.Domain.Services;
using VenueBooking.Domain.Shared;

namespace VenueBooking.BusinessLogic.Services;

public sealed class BookingService(
    IVenueRepository venueRepository,
    IBookingRepository bookingRepository,
    IPricingRuleRepository pricingRuleRepository,
    RentalPriceCalculator priceCalculator) : IBookingService
{
    public async Task<Result<BookingConfirmation>> BookAsync(
        BookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidatePeriod(request);
        if (validation.IsFailure) return validation.Error;

        var venue = await venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null) return VenueErrors.NotFound(request.VenueId);

        // перевірка на наявність бронювань для даного залу в заданий період
        if (await bookingRepository.HasOverlapAsync(request.VenueId, request.StartUtc, request.EndUtc,
                cancellationToken))
            return BookingErrors.VenueAlreadyBooked;

        // отримання існуючих послуг з каталогу за їхніми ідентифікаторами
        var servicesResult = ResolveServices(venue, request.ServiceIds);
        if (servicesResult.IsFailure) return servicesResult.Error;

        var pricingRules = await pricingRuleRepository.GetAllAsync(cancellationToken);

        // розрахунок вартості оренди залу на основі базової ціни та правил ціноутворення
        var rentalCost = priceCalculator.CalculateRentalCost(
            venue.BasePricePerHour,
            TimeOnly.FromDateTime(request.StartUtc),
            TimeOnly.FromDateTime(request.EndUtc),
            pricingRules);

        var selectedServices = servicesResult.Value;

        Booking booking = new()
        {
            VenueId = request.VenueId,
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            CustomerName = request.CustomerName,
            Status = BookingStatus.Confirmed,
            RentalCost = rentalCost,
            ServicesCost = selectedServices.Sum(service => service.Price),
            Items = selectedServices
                .Select(service => new BookingItem
                {
                    ServiceId = service.Id,
                    ServiceName = service.Name,
                    Price = service.Price
                })
                .ToList()
        };

        await bookingRepository.AddAsync(booking, cancellationToken);

        return new BookingConfirmation(booking.Id, booking.RentalCost, booking.ServicesCost, booking.TotalCost);
    }

    // Перевірка правильності періоду бронювання
    private static Result ValidatePeriod(BookingRequest request)
    {
        if (request.EndUtc <= request.StartUtc) return BookingErrors.InvalidPeriod;

        if (request.StartUtc < DateTime.UtcNow) return BookingErrors.PeriodInThePast;

        if (DateOnly.FromDateTime(request.StartUtc) != DateOnly.FromDateTime(request.EndUtc))
            return BookingErrors.PeriodSpansMultipleDays;

        return Result.Success();
    }

    private static Result<IReadOnlyList<Service>> ResolveServices(Venue venue, IReadOnlyList<Guid>? serviceIds)
        => RequestedServices.Match(serviceIds, venue.Services, BookingErrors.ServiceNotOfferedByVenue);
}