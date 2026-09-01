namespace VenueBooking.Api.DTOs.Response;

public record VenueResponseDto(
    Guid Id,
    string Name,
    int Capacity,
    decimal BasePricePerHour,
    IReadOnlyList<ServiceResponseDto> Services);