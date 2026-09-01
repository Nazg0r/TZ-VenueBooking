using VenueBooking.Domain.Models;

namespace VenueBooking.Domain.Services;

// Розбиває період бронювання на відрізки по межах цінових правил і застосовує
// до кожного сегмента правило з найвищим пріоритетом, при відсутності правила — базова ставка.
public sealed class RentalPriceCalculator
{
    public decimal CalculateRentalCost(
        decimal basePricePerHour,
        TimeOnly start,
        TimeOnly end,
        IReadOnlyCollection<PricingRule> rules)
    {
        var boundaries = new SortedSet<TimeOnly> { start, end };

        foreach (var rule in rules)
        {
            if (rule.StartTime > start && rule.StartTime < end) boundaries.Add(rule.StartTime);

            if (rule.EndTime > start && rule.EndTime < end) boundaries.Add(rule.EndTime);
        }

        var points = boundaries.ToArray();
        var total = 0m;

        for (var i = 0; i < points.Length - 1; i++)
        {
            var segmentHours = (decimal)(points[i + 1] - points[i]).TotalHours;
            var multiplier = ResolveMultiplier(points[i], points[i + 1], rules);

            total += basePricePerHour * segmentHours * multiplier;
        }

        return decimal.Round(total, 2);
    }

    private static decimal ResolveMultiplier(
        TimeOnly segmentStart,
        TimeOnly segmentEnd,
        IReadOnlyCollection<PricingRule> rules)
    {
        return rules
            .Where(rule => rule.StartTime <= segmentStart && rule.EndTime >= segmentEnd)
            .OrderByDescending(rule => rule.Priority)
            .Select(rule => rule.Multiplier)
            .DefaultIfEmpty(1m)
            .First();
    }
}