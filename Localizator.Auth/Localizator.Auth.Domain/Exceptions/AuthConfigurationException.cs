namespace Localizator.Auth.Domain.Exceptions;

public sealed class AuthConfigurationException : Exception
{
    public AuthConfigurationException()
    {
    }

    public AuthConfigurationException(string message)
        : base(message)
    {
    }

    public AuthConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
