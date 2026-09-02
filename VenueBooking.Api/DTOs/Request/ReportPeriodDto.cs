namespace VenueBooking.Api.DTOs.Request;

public record ReportPeriodDto(
    DateTime? FromUtc,
    DateTime? ToUtc);
