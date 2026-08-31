namespace VenueBooking.Domain.Models;

// Модель фіксованої послуги бронювання
public class BookingItem
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}