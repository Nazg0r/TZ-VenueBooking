using Microsoft.EntityFrameworkCore;

using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Data;

public class VenueBookingContext(DbContextOptions<VenueBookingContext> options) : DbContext(options)
{
    public DbSet<Venue> Venues { get; set; } = null!;

    public DbSet<Service> Services { get; set; } = null!;

    public DbSet<Booking> Bookings { get; set; } = null!;

    public DbSet<PricingRule> PricingRules { get; set; } = null!;

    public DbSet<BookingItem> BookingItems { get; set; } = null!;

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    // Задання глобальної точності для всіх властивостей decimal у моделях
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Venue)
            .WithMany(v => v.Bookings)
            .HasForeignKey(b => b.VenueId);

        modelBuilder.Entity<Venue>()
            .HasMany(v => v.Services)
            .WithMany()
            .UsingEntity("VenueServices");

        modelBuilder.Entity<PricingRule>()
            .Property(p => p.Multiplier)
            .HasPrecision(4, 3); // Обмеження точності для множника

        // Індекс для пошуку доступних залів
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.VenueId, b.StartUtc, b.EndUtc });
    }

    // Фіксація часу створення та оновлення записів
    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAtUtc = now;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAtUtc = now;
        }
    }
}
