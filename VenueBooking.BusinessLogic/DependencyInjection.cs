using Microsoft.Extensions.DependencyInjection;

using VenueBooking.BusinessLogic.Services;
using VenueBooking.Domain.Interfaces.Services;
using VenueBooking.Domain.Services;

namespace VenueBooking.BusinessLogic;

public static class DependencyInjection
{
    // Розширення, яке агрегує підключення сервісів у контейнері DI
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<RentalPriceCalculator>();

        services.AddScoped<IVenueService, VenueService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}