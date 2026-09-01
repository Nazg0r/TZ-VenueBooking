namespace VenueBooking.Domain.Contracts.Reports;

// Виручка за період: сумарно та по кожному залу окремо.
public sealed record RevenueReport(
    decimal RentalRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue,
    int Bookings,
    IReadOnlyList<VenueRevenue> ByVenue);

public sealed record VenueRevenue(
    VenueRef Venue,
    int Bookings,
    decimal RentalRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);
