using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VenueBooking.DataAccess.Data;
using VenueBooking.DataAccess.Repositories;
using VenueBooking.Domain.Interfaces.Repositories;

namespace VenueBooking.DataAccess;

public static class DependencyInjection
{
    // Розширення, яке агрегує підключення бази дани та реєстрацію репозиторіїв у контейнері DI
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<VenueBookingContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();

        return services;
    }
}
