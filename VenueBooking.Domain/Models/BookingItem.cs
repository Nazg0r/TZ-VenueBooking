namespace VenueBooking.Domain.Models;

public class BookingItem : Entity
{
    public Guid BookingId { get; set; }

    public Guid ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public decimal Price { get; set; }
}
