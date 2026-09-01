using System.ComponentModel.DataAnnotations;

namespace VenueBooking.Api.DTOs.Request;

public record BookDto(
    Guid VenueId,
    [property: Required, MaxLength(200)] string CustomerName,
    DateTime StartUtc,
    DateTime EndUtc,
    IReadOnlyList<Guid>? ServiceIds);