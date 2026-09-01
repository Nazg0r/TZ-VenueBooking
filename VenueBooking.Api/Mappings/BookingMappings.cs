using VenueBooking.Api.DTOs.Request;
using VenueBooking.Domain.Contracts;

namespace VenueBooking.Api.Mappings;

// Клас з розширеннями для перетворень DTO бронювань у доменні об'єкти та навпаки
public static class BookingMappings
{
    public static BookingRequest ToRequest(this BookDto dto)
        => new(dto.VenueId, dto.StartUtc, dto.EndUtc, dto.CustomerName, dto.ServiceIds);
}