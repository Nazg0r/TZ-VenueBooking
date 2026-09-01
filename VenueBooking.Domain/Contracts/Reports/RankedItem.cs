namespace VenueBooking.Domain.Contracts.Reports;

// Позиція рейтингу, яка включає сутність, кількість і частку від загалу.
public sealed record RankedItem<T>(T Item, int Count, decimal Share);
