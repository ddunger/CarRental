namespace CarRental.Domain.Results
{
    public class RepositoryResult<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public Exception? Exception { get; }

        private RepositoryResult(bool isSuccess, T? value, string? error, Exception? ex)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = error;
            Exception = ex;
        }

        public static RepositoryResult<T> Success(T value) => new(true, value, null, null);
        public static RepositoryResult<T> Failure(string error, Exception? ex = null) => new(false, default, error, ex);
    }
}
