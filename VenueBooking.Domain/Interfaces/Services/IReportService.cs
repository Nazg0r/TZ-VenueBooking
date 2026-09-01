using VenueBooking.Domain.Contracts.Reports;
using VenueBooking.Domain.Shared;

namespace VenueBooking.Domain.Interfaces.Services;

public interface IReportService
{
    Task<Result<IReadOnlyList<RankedItem<VenueRef>>>> GetVenuePopularityAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RankedItem<ServiceRef>>>> GetServicePopularityAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<HourDemand>>> GetHourDemandAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default);

    Task<Result<RevenueReport>> GetRevenueAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default);

    Task<Result<OccupancyReport>> GetOccupancyAsync(
        ReportFilter filter,
        CancellationToken cancellationToken = default);
}
