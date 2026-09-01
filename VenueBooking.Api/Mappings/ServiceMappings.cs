using VenueBooking.Api.DTOs.Response;
using VenueBooking.Domain.Models;

namespace VenueBooking.Api.Mappings;

// Клас з розширеннями для перетворень доменних послуг у DTO
public static class ServiceMappings
{
    public static ServiceResponseDto ToResponse(this Service service)
        => new(service.Id, service.Name, service.Price);
}
