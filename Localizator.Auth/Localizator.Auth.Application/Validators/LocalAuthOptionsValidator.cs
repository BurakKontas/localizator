using FluentValidation;
using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Application.Validators;

public sealed class LocalAuthOptionsValidator : AbstractValidator<LocalAuthOptions>
{
    public LocalAuthOptionsValidator()
    {
        RuleFor(x => x.AdminUser)
            .NotEmpty().WithMessage("LOCAL_ADMIN_USERNAME is required");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("LOCAL_ADMIN_PASSWORD is required")
            .MinimumLength(8);
    }
}
