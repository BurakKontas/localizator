using Localizator.Shared.Extensions;
using Localizator.Shared.Helpers;
using Localizator.Shared.Resources;
using Soenneker.Dtos.ProblemDetails;

namespace Localizator.Shared.Exceptions;

public abstract class BaseException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    protected BaseException(
        string code,
        int statusCode,
        Exception? innerException = null, string? message = null)
        : base(message ?? GetMessage(code) ?? Errors.AnErrorOccured, innerException)
    {
        Code = code;
        StatusCode = statusCode;
    }

    protected virtual string Title => "Error";

    protected virtual IDictionary<string, object> Extensions => new Dictionary<string, object>
    {
        ["code"] = Code
    };

    public virtual ProblemDetailsDto GetProblemDetails()
    {
        ProblemDetailsDto details = new()
        {
            Title = Title,
            Detail = Message,
            Status = StatusCode,
            Type = HttpProblemTypeMapper.FromStatusCode(StatusCode),
            Extensions = { }
        };

        details.Extensions!.Concat(Extensions);

        return details;
    }

    public static string GetMessage(string code)
    {
        return Errors.ResourceManager.GetString(code) ?? Errors.AnErrorOccured;
    }
}
