using CarRental.Domain.Enums;

namespace CarRental.Application.Common.Results
{
    public record Result<T>
    {
        public T? Value { get; init; }
        public string? Error { get; init; }
        public ResultError ErrorType { get; init; } = ResultError.None;
        public bool IsSuccess => ErrorType == ResultError.None;

        public static Result<T> Success(T value) => new() { Value = value };
        public static Result<T> Failure(string error, ResultError type) => new() { Error = error, ErrorType = type };
    }

    public record Result
    {
        public string? Error { get; init; }
        public ResultError ErrorType { get; init; } = ResultError.None;
        public bool IsSuccess => ErrorType == ResultError.None;

        public static Result Success() => new();
        public static Result Failure(string error, ResultError type) => new() { Error = error, ErrorType = type };
    }
}
