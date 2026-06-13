using CarRental.Domain.Enums;

namespace CarRental.Domain.Results
{
    public record ApplicationResult<T>
    {
        public T? Value { get; init; }
        public string? Error { get; init; }
        public ResultError ErrorType { get; init; } = ResultError.None;
        public bool IsSuccess => ErrorType == ResultError.None;

        public static ApplicationResult<T> Success(T value) => new() { Value = value };
        public static ApplicationResult<T> Failure(string error, ResultError type) => new() { Error = error, ErrorType = type };
    }

    public record ApplicationResult
    {
        public string? Error { get; init; }
        public ResultError ErrorType { get; init; } = ResultError.None;
        public bool IsSuccess => ErrorType == ResultError.None;

        public static ApplicationResult Success() => new();
        public static ApplicationResult Failure(string error, ResultError type) => new() { Error = error, ErrorType = type };
    }
}
