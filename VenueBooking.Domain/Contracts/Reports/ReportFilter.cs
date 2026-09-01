namespace VenueBooking.Domain.Contracts.Reports;

// Спільний фільтр звітів
public sealed record ReportFilter(
    Guid? VenueId,
    DateTime? FromUtc,
    DateTime? ToUtc);
