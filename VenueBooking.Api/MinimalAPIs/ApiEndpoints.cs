namespace VenueBooking.Api.MinimalAPIs;

public static class ApiEndpoints
{
    // Розширення, яке агрегує усі групи маршрутів.
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapVenueEndpoints();
        app.MapBookingEndpoints();

        return app;
    }
}