using System.ComponentModel.DataAnnotations;

namespace VenueBooking.Api.DTOs.Request;

public record VenueUpdateDto(
    [property: Required, MaxLength(200)] string Name,
    [property: Range(1, 100_000)] int Capacity,
    [property: Range(0.01, 1_000_000)] decimal BasePricePerHour);