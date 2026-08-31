namespace VenueBooking.Domain.Models;

// Модель правила ціноутворення для певного часового проміжку
public class PricingRule : Entity
{
    public required string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public decimal Multiplier { get; set; } = 1m;
    // Для часових проміжків, які накладаються
    public int Priority { get; set; }
}