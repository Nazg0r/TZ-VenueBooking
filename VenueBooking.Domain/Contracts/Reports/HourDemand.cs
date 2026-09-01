namespace VenueBooking.Domain.Contracts.Reports;

// Попит за годиною доби, яка включає кількість бронювань, які зачіпають цю годину, і кількість годин, які заброньовано.
public sealed record HourDemand(int Hour, int Bookings, double BookedHours);
