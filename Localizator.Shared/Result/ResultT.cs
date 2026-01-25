using Localizator.Shared.Providers;

namespace Localizator.Shared.Result;

public class Result<T> : Result
{
    public new T? Data { get; set; }

    public static Result<T> Success(T? data = default, string message = "", Meta? meta = null)
        => new Result<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Meta = meta ?? MetaProvider.Get()
        };

    public static Result<T> Failure(string message = "", T? data = default, Meta? meta = null)
        => new Result<T>
        {
            IsSuccess = false,
            Message = message,
            Data = data,
            Meta = meta ?? MetaProvider.Get()
        };
}