namespace VenueBooking.Api.DTOs.Request;

public record ReportFilterDto(
    Guid? VenueId,
    DateTime? FromUtc,
    DateTime? ToUtc);
