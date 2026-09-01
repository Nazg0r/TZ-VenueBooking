namespace VenueBooking.Domain.Contracts.Reports;

// Короткі посилання на сутності для рядків звітів.
public sealed record VenueRef(Guid Id, string Name);

public sealed record ServiceRef(Guid Id, string Name);
