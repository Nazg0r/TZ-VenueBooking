using Microsoft.EntityFrameworkCore;

using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Data;

public class VenueBookingContext(DbContextOptions<VenueBookingContext> options) : DbContext(options)
{
    public DbSet<Venue> Venues { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<PricingRule> PricingRules { get; set; } = null!;

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

    // Єдина точність для всіх грошових decimal-полів.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Venue)
            .WithMany()
            .HasForeignKey(booking => booking.VenueId);

        modelBuilder.Entity<Booking>()
            .OwnsMany(booking => booking.Items, item => item.ToTable("BookingItems"));

        modelBuilder.Entity<Venue>()
            .HasMany(venue => venue.Services)
            .WithMany()
            .UsingEntity("VenueServices");

        modelBuilder.Entity<PricingRule>()
            .Property(rule => rule.Multiplier)
            .HasPrecision(4, 3);

        // Індекс під пошук вільних залів за інтервалом.
        modelBuilder.Entity<Booking>()
            .HasIndex(booking => new { booking.VenueId, booking.StartUtc, booking.EndUtc });
    }

    // Проставляє дати створення та оновлення сутностей.
    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entity>())
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAtUtc = now;
            else if (entry.State == EntityState.Modified) entry.Entity.UpdatedAtUtc = now;
    }
}