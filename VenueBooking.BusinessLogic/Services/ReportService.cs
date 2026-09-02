using VenueBooking.Domain.Contracts.Reports;
using VenueBooking.Domain.Errors;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Interfaces.Services;
using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.BusinessLogic.Services;

public sealed class ReportService(
    IVenueRepository venueRepository,
    IBookingRepository bookingRepository,
    IPricingRuleRepository pricingRuleRepository) : IReportService
{
    // Звіт, який повертає популярність залів за кількістю бронювань у заданому періоді
    public async Task<Result<IReadOnlyList<RankedItem<VenueRef>>>> GetVenuePopularityAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        // Завантаження бронювань за заданим періодом
        var bookingsResult = await LoadBookingsAsync(new ReportFilter(null, fromUtc, toUtc), cancellationToken);
        if (bookingsResult.IsFailure) return bookingsResult.Error;

        // Отримання назв залів
        var venueNames = await GetVenueNamesAsync(cancellationToken);

        // Формування рейтингу залів за кількістю бронювань
        var ranked = Rank(
            bookingsResult.Value.GroupBy(booking => booking.VenueId),
            group => VenueRefFor(venueNames, group.Key));

        return Result<IReadOnlyList<RankedItem<VenueRef>>>.Success(ranked);
    }

    // Звіт, який повертає популярність послуг за кількістю замовлень у заданому періоді
    public async Task<Result<IReadOnlyList<RankedItem<ServiceRef>>>> GetServicePopularityAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Завантаження бронювань за заданим фільтром
        var bookingsResult = await LoadBookingsAsync(filter, cancellationToken);
        if (bookingsResult.IsFailure) return bookingsResult.Error;

        // Отримання всіх замовлених послуг з бронювань
        var items = bookingsResult.Value.SelectMany(booking => booking.Items).ToList();

        // Формування рейтингу послуг за кількістю замовлень
        var ranked = Rank(
            items.GroupBy(item => item.ServiceId),
            group => new ServiceRef(group.Key, group.First().ServiceName));

        return Result<IReadOnlyList<RankedItem<ServiceRef>>>.Success(ranked);
    }

    // Звіт, який повертає попит на години бронювання у заданому періоді
    public async Task<Result<IReadOnlyList<HourDemand>>> GetHourDemandAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Завантаження бронювань за заданим фільтром
        var bookingsResult = await LoadBookingsAsync(filter, cancellationToken);
        if (bookingsResult.IsFailure) return bookingsResult.Error;

        // Отримання робочих годин залів
        var (_, openHour, closeHour) = await GetWorkingHoursAsync(cancellationToken);
        var bookings = bookingsResult.Value;

        var demand = new List<HourDemand>(closeHour - openHour);
        // Визначення заброньованого часу для кожної години робочого дня
        for (var hour = openHour; hour < closeHour; hour++)
        {
            var bookedHours = 0d;

            foreach (var booking in bookings)
            {
                // Обчислення перекриття бронювання з годиною робочого дня
                var overlap = Math.Min(booking.EndUtc.TimeOfDay.TotalHours, hour + 1)
                              - Math.Max(booking.StartUtc.TimeOfDay.TotalHours, hour);

                if (overlap > 0) bookedHours += overlap;
            }

            demand.Add(new HourDemand(hour, Math.Round(bookedHours, 2)));
        }

        return Result<IReadOnlyList<HourDemand>>.Success(demand);
    }

    // Звіт, який повертає фінансові показники за заданим періодом
    public async Task<Result<RevenueReport>> GetRevenueAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Завантаження бронювань за заданим фільтром
        var bookingsResult = await LoadBookingsAsync(filter, cancellationToken);
        if (bookingsResult.IsFailure) return bookingsResult.Error;

        // Отримання назв залів
        var venueNames = await GetVenueNamesAsync(cancellationToken);
        var bookings = bookingsResult.Value;

        // Формування фінансових показників для кожного залу
        var venuesRevenue = bookings
            .GroupBy(booking => booking.VenueId)
            .Select(group => new VenueRevenue(
                VenueRefFor(venueNames, group.Key),
                group.Count(),
                group.Sum(booking => booking.RentalCost),
                group.Sum(booking => booking.ServicesCost),
                group.Sum(booking => booking.TotalCost)))
            .OrderByDescending(venue => venue.TotalRevenue)
            .ToList();

        // Формування загального фінансового звіту
        var report = new RevenueReport(
            venuesRevenue.Sum(venue => venue.RentalRevenue),
            venuesRevenue.Sum(venue => venue.ServicesRevenue),
            venuesRevenue.Sum(venue => venue.TotalRevenue),
            bookings.Count,
            venuesRevenue);

        return Result<RevenueReport>.Success(report);
    }

    // Звіт, який повертає завантаженість залів за заданим періодом
    public async Task<Result<OccupancyReport>> GetOccupancyAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Завантаження бронювань за заданим фільтром
        var bookingsResult = await LoadBookingsAsync(filter, cancellationToken);
        if (bookingsResult.IsFailure) return bookingsResult.Error;

        // Отримання назв залів
        var venueNames = await GetVenueNamesAsync(cancellationToken);
        // Отримання кількості робочих годин на день
        var (operatingHoursPerDay, _, _) = await GetWorkingHoursAsync(cancellationToken);

        var bookings = bookingsResult.Value;

        var days = CountDays(filter.FromUtc, filter.ToUtc, bookings);
        var availableHours = operatingHoursPerDay * days;

        var venues = bookings
            .GroupBy(booking => booking.VenueId)
            .Select(group =>
            {
                // Загальна кількість заброньованих годин залу
                var bookedHours = group.Sum(booking => (booking.EndUtc - booking.StartUtc).TotalHours);
                var rate = availableHours <= 0
                    ? 0m
                    : Math.Round((decimal)(bookedHours / availableHours), 4);

                // Формування завантаженості залу
                return new VenueOccupancy(
                    VenueRefFor(venueNames, group.Key),
                    group.Count(),
                    Math.Round(bookedHours, 2),
                    Math.Round(availableHours, 2),
                    rate,
                    group.Sum(booking => booking.TotalCost));
            })
            .OrderByDescending(venue => venue.OccupancyRate)
            .ToList();

        // Формування загального звіту про завантаженість залів
        var report = new OccupancyReport(Math.Round(operatingHoursPerDay, 2), venues);

        return Result<OccupancyReport>.Success(report);
    }

    // Валідує фільтр і завантажує активні бронювання за ним
    private async Task<Result<IReadOnlyList<Booking>>> LoadBookingsAsync(
        ReportFilter filter,
        CancellationToken cancellationToken)
    {
        if (filter is { FromUtc: { } from, ToUtc: { } to } && to <= from) return ReportErrors.InvalidPeriod;

        if (filter.VenueId is { } venueId
            && await venueRepository.GetByIdAsync(venueId, cancellationToken) is null)
            return VenueErrors.NotFound(venueId);

        var bookings = await bookingRepository.GetForReportsAsync(
            filter.VenueId, filter.FromUtc, filter.ToUtc, cancellationToken);

        return Result<IReadOnlyList<Booking>>.Success(bookings);
    }

    // Формує рейтинг елементів за кількістю появ у групах, обчислює частку від загальної кількості
    private static IReadOnlyList<RankedItem<TRef>> Rank<TKey, TElement, TRef>(
        IEnumerable<IGrouping<TKey, TElement>> groups,
        Func<IGrouping<TKey, TElement>, TRef> refSelector)
    {
        // Підрахунок кількості елементів у кожній групі та формування списку з посиланням на елемент та його кількість
        var counted = groups
            .Select(group => (Ref: refSelector(group), Count: group.Count()))
            .ToList();

        var total = counted.Sum(row => row.Count);

        return counted
            .OrderByDescending(row => row.Count)
            .Select(row => new RankedItem<TRef>(
                row.Ref,
                row.Count,
                total == 0 ? 0m : Math.Round((decimal)row.Count / total, 4)))
            .ToList();
    }

    // Повертає робочі години залів на основі правил ціноутворення
    private async Task<(double HoursPerDay, int OpenHour, int CloseHour)> GetWorkingHoursAsync(
        CancellationToken cancellationToken)
    {
        // Отримання всіх правил ціноутворення
        var rules = await pricingRuleRepository.GetAllAsync(cancellationToken);
        if (rules.Count == 0) return (24d, 0, 24);

        var opensAt = rules.Min(rule => rule.StartTime);
        var closesAt = rules.Max(rule => rule.EndTime);

        return (
            (closesAt - opensAt).TotalHours,
            (int)Math.Floor(opensAt.ToTimeSpan().TotalHours),
            (int)Math.Ceiling(closesAt.ToTimeSpan().TotalHours));
    }

    private async Task<IReadOnlyDictionary<Guid, string>> GetVenueNamesAsync(CancellationToken cancellationToken)
    {
        var venues = await venueRepository.GetAllAsync(cancellationToken);
        return venues.ToDictionary(venue => venue.Id, venue => venue.Name);
    }

    private static VenueRef VenueRefFor(IReadOnlyDictionary<Guid, string> venueNames, Guid id)
    {
        return new VenueRef(id, venueNames.GetValueOrDefault(id, "(зал видалено)"));
    }

    // Рахує кількість днів у заданому періоді або у періоді, що охоплює всі бронювання.
    private static int CountDays(DateTime? fromUtc, DateTime? toUtc, IReadOnlyList<Booking> bookings)
    {
        if (bookings.Count == 0) return 0;

        var from = fromUtc ?? bookings.Min(booking => booking.StartUtc);
        var to = toUtc ?? bookings.Max(booking => booking.EndUtc);

        return Math.Max(1, (int)Math.Ceiling((to - from).TotalDays));
    }
}