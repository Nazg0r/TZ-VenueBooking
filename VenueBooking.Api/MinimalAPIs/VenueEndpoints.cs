using VenueBooking.Api.DTOs.Request;
using VenueBooking.Api.Extensions;
using VenueBooking.Api.Mappings;
using VenueBooking.Domain.Interfaces.Services;

namespace VenueBooking.Api.MinimalAPIs;

public static class VenueEndpoints
{
    // Розширення, яке додає маршрути для роботи зі залами
    public static IEndpointRouteBuilder MapVenueEndpoints(this IEndpointRouteBuilder app)
    {
        const string prefix = "api/venues";
        var group = app.MapGroup(prefix).WithTags("Venues");

        group.MapGet("",
            async (IVenueService venueService, CancellationToken cancellationToken) =>
            {
                var result = await venueService.GetAllAsync(cancellationToken);

                return result.ToOk(venues => venues.Select(venue => venue.ToResponse()).ToList());
            });

        group.MapGet("/available",
            async ([AsParameters] FindAvailableVenuesDto dto, IVenueService venueService,
                CancellationToken cancellationToken) =>
            {
                var result = await venueService.FindAvailableAsync(dto.ToRequest(), cancellationToken);

                return result.ToOk(venues => venues.Select(venue => venue.ToResponse()).ToList());
            })
            .ProducesValidationProblem();

        group.MapPost("",
            async (VenueCreationDto dto, IVenueService venueService, CancellationToken cancellationToken) =>
            {
                var result = await venueService.AddNewAsync(dto.ToModel(), dto.ServiceIds, cancellationToken);

                return result.ToCreated(venue => $"/{prefix}/{venue.Id}", venue => venue.ToResponse());
            })
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}",
            async (Guid id, VenueUpdateDto dto, IVenueService venueService, CancellationToken cancellationToken) =>
            {
                var result = await venueService.UpdateAsync(dto.ToModel(id), dto.ServiceIds, cancellationToken);

                return result.ToNoContent();
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}",
            async (Guid id, IVenueService venueService, CancellationToken cancellationToken) =>
            {
                var result = await venueService.DeleteAsync(id, cancellationToken);

                return result.ToNoContent();
            })
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}