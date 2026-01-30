namespace Localizator.Shared.Exceptions;

public sealed class TechnicalException : BaseException
{
    protected override string Title => "Technical Error";

    public TechnicalException(
        string code = "TechnicalError",
        Exception? innerException = null, string? message = null)
        : base(code, 500, innerException, message)
    {
    }
}
