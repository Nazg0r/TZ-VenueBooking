using VenueBooking.Api.DTOs.Request;
using VenueBooking.Api.DTOs.Response;
using VenueBooking.Domain.Contracts;
using VenueBooking.Domain.Models;

namespace VenueBooking.Api.Mappings;

// Клас з розширеннями для перетворень DTO залів у доменні об'єкти та навпаки
public static class VenueMappings
{
    public static AvailableVenuesRequest ToRequest(this FindAvailableVenuesDto dto)
        => new(dto.StartUtc, dto.EndUtc, dto.Capacity);

    public static Venue ToModel(this VenueCreationDto dto)
        => new()
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            BasePricePerHour = dto.BasePricePerHour
        };

    public static Venue ToModel(this VenueUpdateDto dto, Guid id)
        => new()
        {
            Id = id,
            Name = dto.Name,
            Capacity = dto.Capacity,
            BasePricePerHour = dto.BasePricePerHour
        };

    public static VenueResponseDto ToResponse(this Venue venue)
        => new(
            venue.Id,
            venue.Name,
            venue.Capacity,
            venue.BasePricePerHour,
            venue.Services
                .Select(service => new ServiceResponseDto(service.Id, service.Name, service.Price))
                .ToList());
}