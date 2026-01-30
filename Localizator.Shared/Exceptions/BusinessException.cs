namespace Localizator.Shared.Exceptions;

public sealed class BusinessException : BaseException
{
    protected override string Title => "Business Error";

    public BusinessException(string message)
        : this("NoErrorCode", message: message)
    {
    }

    public BusinessException(
        string code,
        int statusCode = 400,
        Exception? innerException = null, string? message = null)
        : base(code, statusCode, innerException, message)
    {
    }
}