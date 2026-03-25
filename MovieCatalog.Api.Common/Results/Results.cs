namespace MovieCatalog.Api.Common.Results;


public readonly record struct Error(string Code, string Description)
{
    public static readonly Error None = new("", "");
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
}

public readonly record struct Result
{
    public bool IsSuccess { get; }
    public Error[] Errors { get; }

    private Result(bool isSucess, Error[] errors)
      => (IsSuccess, Errors) = (isSucess, errors);


    public static Result Success() => new(true, Array.Empty<Error>());
    public static Result Failure(params Error[] errors) => new(false, errors);
    public static Result NotFound(params Error[] errors) => new(false, errors);
    public static Result BadRequest(params Error[] errors) => new(false, errors);

    public static Result Combine(params Result[] results)
        => results.Any(r => !r.IsSuccess)
            ? Failure(results.Where(r => !r.IsSuccess).SelectMany(r => r.Errors).ToArray())
            : Success();
}

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error[] Errors { get; }

    private Result(bool isSucess, T? value, Error[] errors)
      => (IsSuccess, Value, Errors) = (isSucess, value, errors);

    public static Result<T> Success(T value) => new(true, value, Array.Empty<Error>());
    public static Result<T> Failure(params Error[] errors) => new(false, default, errors);
    public static Result<T> NotFound(params Error[] errors) => new(false, default, errors);
    public static Result<T> BadRequest() => new(false, default, []);
    public static Result<T> BadRequest(params Error[] errors) => new(false, default, errors);

    public Result<K> Map<K>(Func<T, K> map)
        => IsSuccess
            ? Result<K>.Success(map(Value!))
            : Result<K>.Failure(Errors);
    public Result<K> Bind<K>(Func<T, Result<K>> next)
        => IsSuccess
            ? next(Value!)
            : Result<K>.Failure(Errors);
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
        => IsSuccess && !predicate(Value!)
            ? Failure(error)
            : this;
}
