using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Errors;

public static class ReportErrors
{
    public static readonly Error InvalidPeriod = Error.Validation(
        "Reports.InvalidPeriod", "The report period end must be later than its start.");
}
