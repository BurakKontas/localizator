using FluentValidation;
using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Application.Validators;

public sealed class HybridAuthOptionsValidator : AbstractValidator<HybridAuthOptions>
{
    public HybridAuthOptionsValidator()
    {
        RuleFor(x => x.ApiKeys)
            .NotEmpty().WithMessage("API_KEY is required");

        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("OIDC_ISSUER is required");

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("OIDC_CLIENT_ID is required");

        RuleFor(x => x.ClientSecret)
            .NotEmpty()
            .WithMessage("OIDC_CLIENT_SECRET is required");

        RuleFor(x => x.RedirectUri)
            .NotEmpty()
            .WithMessage("OIDC_REDIRECT_URI is required")
            .Must(BeAValidUri)
            .WithMessage("OIDC_REDIRECT_URI must be a valid absolute URI");
    }

    private static bool BeAValidUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out _);
}