namespace VenueBooking.Api.DTOs.Response;

public record BookingItemResponseDto(Guid ServiceId, string ServiceName, decimal Price);
