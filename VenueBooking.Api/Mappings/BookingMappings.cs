using VenueBooking.Api.DTOs.Request;
using VenueBooking.Api.DTOs.Response;
using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Models;

namespace VenueBooking.Api.Mappings;

// Клас з розширеннями для перетворень DTO бронювань у доменні об'єкти та навпаки
public static class BookingMappings
{
    public static BookingRequest ToRequest(this BookDto dto)
        => new(dto.VenueId, dto.StartUtc, dto.EndUtc, dto.CustomerName, dto.ServiceIds);

    public static BookingResponseDto ToResponse(this Booking booking)
        => new(
            booking.Id,
            booking.VenueId,
            booking.StartUtc,
            booking.EndUtc,
            booking.CustomerName,
            booking.Status.ToString(),
            booking.RentalCost,
            booking.ServicesCost,
            booking.TotalCost,
            booking.Items
                .Select(item => new BookingItemResponseDto(item.ServiceId, item.ServiceName, item.Price))
                .ToList());
}
