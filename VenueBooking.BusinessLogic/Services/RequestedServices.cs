using VenueBooking.Domain.Models;
using VenueBooking.Domain.Shared;

namespace VenueBooking.BusinessLogic.Services;

// Допоміжний сервіс, який вміщує спільну логіку інших сервісів
internal static class RequestedServices
{
    // Статичний метод для зіставлення запитаних і доступних послуг
    public static Result<IReadOnlyList<Service>> Match(
        IReadOnlyList<Guid>? requestedIds,
        IEnumerable<Service> available,
        Func<Guid, Error> unknownIdError)
    {
        if (requestedIds is null || requestedIds.Count == 0) return Result<IReadOnlyList<Service>>.Success([]);

        var distinctIds = requestedIds.Distinct().ToArray();
        var matched = available.Where(service => distinctIds.Contains(service.Id)).ToArray();

        var missing = distinctIds.Except(matched.Select(service => service.Id)).ToArray();
        if (missing.Length > 0) return unknownIdError(missing[0]);

        return Result<IReadOnlyList<Service>>.Success(matched);
    }
}