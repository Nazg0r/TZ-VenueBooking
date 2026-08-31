using VenueBooking.Domain.Enums;

namespace VenueBooking.Domain.Models;

public class Booking : Entity
{
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    // Статус для відстеження стану бронювання
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    // Послуги з фіксованими цінами на момент замовлення
    public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
    // Вартості З врахуванням часових коефіцієнтів
    public decimal RentalCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost => RentalCost + ServicesCost;
}