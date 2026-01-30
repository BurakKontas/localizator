using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Localizator.Shared.Exceptions;

public sealed class TechnicalException : BaseException
{
    protected override string Title { get; }

    public TechnicalException(
        string code = "TechnicalError",
        int statusCode = 500,
        Exception? innerException = null, string? message = null, string? title = null)
        : base(code, statusCode, innerException, message)
    {
        Title = title ?? "Technical Error";
    }
}
