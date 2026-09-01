using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess;

public static class DatabaseInitializer
{
    // Наповнює базу початковими даними
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<VenueBookingContext>();

        await context.Database.MigrateAsync();

        if (await context.PricingRules.AnyAsync()) return;

        var projector = new Service { Name = "Проєктор", Price = 500m };
        var wifi = new Service { Name = "Wi-Fi", Price = 300m };
        var sound = new Service { Name = "Звук", Price = 700m };

        context.PricingRules.AddRange(
            new PricingRule
            {
                Name = "Стандартні години",
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(18, 0),
                Multiplier = 1.00m
            },
            new PricingRule
            {
                Name = "Вечірні години",
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(23, 0),
                Multiplier = 0.80m
            },
            new PricingRule
            {
                Name = "Ранкові години",
                StartTime = new TimeOnly(6, 0),
                EndTime = new TimeOnly(9, 0),
                Multiplier = 0.90m
            },
            new PricingRule
            {
                Name = "Пікові години",
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(14, 0),
                Multiplier = 1.15m,
                Priority = 1
            });

        context.Venues.AddRange(
            new Venue { Name = "Зал A", Capacity = 50, BasePricePerHour = 2000m, Services = { projector, wifi } },
            new Venue
            {
                Name = "Зал B",
                Capacity = 100,
                BasePricePerHour = 3500m,
                Services = { projector, wifi, sound }
            },
            new Venue { Name = "Зал C", Capacity = 30, BasePricePerHour = 1500m, Services = { wifi } });

        await context.SaveChangesAsync();
    }
}