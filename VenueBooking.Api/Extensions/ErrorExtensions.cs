using Microsoft.AspNetCore.Http.HttpResults;

using VenueBooking.Domain.Shared;

namespace VenueBooking.Api.Extensions;

public static class ErrorExtensions
{
    // Формує TypedResults з ProblemDetails на основі даних Error
    public static ProblemHttpResult ToProblem(this Error error)
    {
        // Визначення статусного коду на основі типу помилки
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            detail: error.Description,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code });
    }
}