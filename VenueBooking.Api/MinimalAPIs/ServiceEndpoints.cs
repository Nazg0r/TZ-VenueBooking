using VenueBooking.Api.Extensions;
using VenueBooking.Api.Mappings;
using VenueBooking.Domain.Interfaces.Services;

namespace VenueBooking.Api.MinimalAPIs;

public static class ServiceEndpoints
{
    // Розширення, яке додає маршрути каталогу послуг
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/services").WithTags("Services");

        group.MapGet("",
            async (IServiceService serviceService, CancellationToken cancellationToken) =>
            {
                var result = await serviceService.GetAllAsync(cancellationToken);

                return result.ToOk(services => services.Select(service => service.ToResponse()).ToList());
            });

        return app;
    }
}
