using FluentValidation;
using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Application.Validators;

public sealed class OidcAuthOptionsValidator : AbstractValidator<OidcAuthOptions>
{
    public OidcAuthOptionsValidator()
    {
        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("OIDC_ISSUER is required");

        RuleFor(x => x.Audience)
            .NotEmpty()
            .WithMessage("OIDC_AUDIENCE is required");

        RuleFor(x => x.ConfigurationUrl)
            .NotEmpty()
            .WithMessage("OIDC_CONFIGURATIONURL is required")
            .Must(BeAValidUri)
            .WithMessage("OIDC_CONFIGURATIONURL must be a valid absolute URI");
    }

    private static bool BeAValidUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out _);
}
