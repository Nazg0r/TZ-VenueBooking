using System.ComponentModel.DataAnnotations;

namespace VenueBooking.Api.DTOs.Request;

public record FindAvailableVenuesDto(
    DateTime StartUtc,
    DateTime EndUtc,
    [property: Range(1, 100_000)] int Capacity);