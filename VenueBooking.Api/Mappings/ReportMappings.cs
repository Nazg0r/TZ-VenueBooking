using VenueBooking.Api.DTOs.Request;
using VenueBooking.Domain.Contracts.Reports;

namespace VenueBooking.Api.Mappings;

// Клас з розширеннями для перетворення DTO звітів у доменні фільтри
public static class ReportMappings
{
    public static ReportFilter ToFilter(this ReportFilterDto dto)
        => new(dto.VenueId, dto.FromUtc, dto.ToUtc);
}
