namespace VenueBooking.Domain.Contracts.Reports;

// Завантаженість залів за період: заброньовані години проти доступних.
public sealed record OccupancyReport(
    double OperatingHoursPerDay,
    IReadOnlyList<VenueOccupancy> Venues);

public sealed record VenueOccupancy(
    VenueRef Venue,
    int Bookings,
    double BookedHours,
    double AvailableHours,
    decimal OccupancyRate,
    decimal Revenue);
