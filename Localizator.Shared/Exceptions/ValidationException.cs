namespace Localizator.Shared.Exceptions;

public sealed class ValidationException : BaseException
{
    public IDictionary<string, string[]> Errors { get; }

    protected override string Title => "Validation Error";

    public ValidationException(
        IDictionary<string, string[]> errors,
        string message = "Validation failed")
        : base("ValidationError", 400, message: message)
    {
        Errors = errors;
    }

    protected override IDictionary<string, object> Extensions
    {
        get
        {
            var extensions = base.Extensions;
            extensions["errors"] = Errors;
            return extensions;
        }
    }
}
