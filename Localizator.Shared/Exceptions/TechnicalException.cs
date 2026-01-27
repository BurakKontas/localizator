namespace Localizator.Shared.Exceptions;

public class TechnicalException : Exception
{
    public string Code { get; }

    public TechnicalException(string message)
        : base(message)
    {
        Code = "NoErrorCode";
    }

    public TechnicalException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public TechnicalException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = innerException.Source ?? "NoErrorCode";
    }

    public TechnicalException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    public TechnicalException(string message, TechnicalException innerException)
        : base(message, innerException)
    {
        Code = innerException.Code;
    }
}