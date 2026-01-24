using FluentValidation;
using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Application.Validators;

public sealed class HeaderAuthOptionsValidator : AbstractValidator<HeaderAuthOptions>
{
    public HeaderAuthOptionsValidator()
    {
        RuleFor(x => x.UserHeader)
            .NotEmpty().WithMessage("User heeader is required");

        RuleFor(x => x.EmailHeader)
            .MinimumLength(3)
            .When(x => !string.IsNullOrEmpty(x.EmailHeader))
            .WithMessage("Email header, if provided, must be at least 3 characters long");
    }
}
