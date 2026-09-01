using VenueBooking.Api.DTOs.Request;
using VenueBooking.Api.Extensions;
using VenueBooking.Api.Mappings;
using VenueBooking.Domain.Interfaces.Services;

namespace VenueBooking.Api.MinimalAPIs;

public static class BookingEndpoints
{
    // Розширення, яке додає маршрути для обробки запитів, пов'язаних із бронюванням.
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        const string prefix = "api/bookings";
        var group = app.MapGroup(prefix).WithTags("Bookings");

        group.MapPost("",
            async (BookDto dto, IBookingService bookingService, CancellationToken cancellationToken) =>
            {
                var result = await bookingService.BookAsync(dto.ToRequest(), cancellationToken);

                return result.ToCreated(
                    confirmation => $"/{prefix}/{confirmation.BookingId}",
                    confirmation => confirmation);
            })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}