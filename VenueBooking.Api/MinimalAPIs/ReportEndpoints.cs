using VenueBooking.Api.DTOs.Request;
using VenueBooking.Api.Extensions;
using VenueBooking.Api.Mappings;
using VenueBooking.Domain.Interfaces.Services;

namespace VenueBooking.Api.MinimalAPIs;

public static class ReportEndpoints
{
    // Розширення, яке додає маршрути звітів та аналітики
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        const string prefix = "api/reports";
        var group = app.MapGroup(prefix).WithTags("Reports");

        group.MapGet("/venue-popularity",
            async ([AsParameters] ReportPeriodDto dto, IReportService reportService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportService.GetVenuePopularityAsync(dto.FromUtc, dto.ToUtc, cancellationToken);

                return result.ToOk(items => items);
            })
            .ProducesValidationProblem();

        group.MapGet("/service-popularity",
            async ([AsParameters] ReportFilterDto dto, IReportService reportService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportService.GetServicePopularityAsync(dto.ToFilter(), cancellationToken);

                return result.ToOk(items => items);
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/hour-demand",
            async ([AsParameters] ReportFilterDto dto, IReportService reportService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportService.GetHourDemandAsync(dto.ToFilter(), cancellationToken);

                return result.ToOk(items => items);
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/revenue",
            async ([AsParameters] ReportFilterDto dto, IReportService reportService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportService.GetRevenueAsync(dto.ToFilter(), cancellationToken);

                return result.ToOk(report => report);
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/occupancy",
            async ([AsParameters] ReportFilterDto dto, IReportService reportService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportService.GetOccupancyAsync(dto.ToFilter(), cancellationToken);

                return result.ToOk(report => report);
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
