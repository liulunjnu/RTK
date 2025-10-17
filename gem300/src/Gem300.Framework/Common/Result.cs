namespace Gem300.Framework.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        protected Result(bool ok, string error)
        {
            IsSuccess = ok;
            Error = error;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Fail(string error) => new Result(false, error);
    }

    public class ResultOf<T> : Result
    {
        public T Value { get; }
        private ResultOf(bool ok, T value, string error) : base(ok, error)
        {
            Value = value;
        }

        public static ResultOf<T> Ok(T value) => new ResultOf<T>(true, value, null);
        public static ResultOf<T> Fail(string error) => new ResultOf<T>(false, default(T), error);
    }
}
