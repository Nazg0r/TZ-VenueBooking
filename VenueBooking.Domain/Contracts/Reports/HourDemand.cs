namespace VenueBooking.Domain.Contracts.Reports;

// Попит за годиною доби: сумарний заброньований час, що припадає на цю годину (по всіх залах).
public sealed record HourDemand(int Hour, double BookedHours);
