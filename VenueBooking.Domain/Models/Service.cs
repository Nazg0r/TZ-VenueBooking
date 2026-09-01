namespace VenueBooking.Domain.Models;

// Модель послуги
public class Service : Entity
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
}