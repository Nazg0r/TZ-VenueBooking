namespace VenueBooking.Domain.Models;

public class Venue : Entity
{
    public required string Name { get; set; }

    public int Capacity { get; set; }

    public decimal BasePricePerHour { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
