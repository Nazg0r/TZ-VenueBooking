namespace VenueBooking.Api.DTOs.Response;

public record BookingResponseDto(
    Guid Id,
    Guid VenueId,
    DateTime StartUtc,
    DateTime EndUtc,
    string CustomerName,
    string Status,
    decimal RentalCost,
    decimal ServicesCost,
    decimal TotalCost,
    IReadOnlyList<BookingItemResponseDto> Items);