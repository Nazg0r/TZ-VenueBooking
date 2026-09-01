using Microsoft.AspNetCore.Http.HttpResults;

using VenueBooking.Domain.Shared;

namespace VenueBooking.Api.Extensions;

// Перетворює Result у типізовану відповідь мінімального API
public static class ResultExtensions
{
    public static Results<Ok<TResponse>, ProblemHttpResult> ToOk<TValue, TResponse>(
        this Result<TValue> result,
        Func<TValue, TResponse> selector)
    {
        if (result.IsFailure) return result.Error.ToProblem();

        return TypedResults.Ok(selector(result.Value));
    }

    public static Results<Created<TResponse>, ProblemHttpResult> ToCreated<TValue, TResponse>(
        this Result<TValue> result,
        Func<TValue, string> location,
        Func<TValue, TResponse> selector)
    {
        if (result.IsFailure) return result.Error.ToProblem();

        return TypedResults.Created(location(result.Value), selector(result.Value));
    }

    public static Results<NoContent, ProblemHttpResult> ToNoContent(this Result result)
    {
        if (result.IsFailure) return result.Error.ToProblem();

        return TypedResults.NoContent();
    }
}