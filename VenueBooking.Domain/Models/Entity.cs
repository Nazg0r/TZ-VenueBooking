namespace VenueBooking.Domain.Models;

// Базова модель сутності
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    // Дата та час створення сутності в UTC
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    // Дата та час оновлення сутності в UTC
    public DateTime? UpdatedAtUtc { get; set; }
}