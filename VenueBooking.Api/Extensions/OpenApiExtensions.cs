using Microsoft.OpenApi;

namespace VenueBooking.Api.Extensions;

public static class OpenApiExtensions
{
    // Підключає OpenAPI з додатковими налаштуваннями схеми типів
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

            options.AddSchemaTransformer((schema, context, _) =>
            {
                var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;

                if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                {
                    schema.Type = JsonSchemaType.Integer;
                    schema.Pattern = null;
                }
                else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                {
                    schema.Type = JsonSchemaType.Number;
                    schema.Pattern = null;
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
