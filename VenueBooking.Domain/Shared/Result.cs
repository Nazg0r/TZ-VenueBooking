using System.Diagnostics.CodeAnalysis;

namespace VenueBooking.Domain.Shared;

// Базова реалізація Result pattern без значення
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new ArgumentException("A successful result cannot contain an error.", nameof(error));

        if (!isSuccess && error == Error.None)
            throw new ArgumentException("A failing result must contain an error.", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess() : onFailure(Error);

    // перевизначення оператору неявного перетворення Error для зручності створення результату
    public static implicit operator Result(Error error) => Failure(error);
}

// Розширена реалізація Result pattern зі значенням
public sealed class Result<T> : Result
{
    internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error) => Value = value;

    [AllowNull]
    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default, false, error);

    public static Result<T> Create(T? value) =>
        value is not null ? Success(value) : Failure(Error.NullValue);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    // перевизначення операторів для зручності створення результатів через неявне перетворення
    public static implicit operator Result<T>(T? value) => Create(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}