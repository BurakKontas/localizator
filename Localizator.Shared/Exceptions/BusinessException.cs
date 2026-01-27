namespace Localizator.Shared.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message)
        : base(message)
    {
        Code = "NoErrorCode";
    }

    public BusinessException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = innerException.Source ?? "NoErrorCode";
    }

    public BusinessException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    public BusinessException(string message, BusinessException innerException)
    : base(message, innerException)
    {
        Code = innerException.Code;
    }
}
