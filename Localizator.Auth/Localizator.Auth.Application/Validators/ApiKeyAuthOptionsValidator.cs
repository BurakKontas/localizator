using FluentValidation;
using Localizator.Auth.Domain.Configuration;

namespace Localizator.Auth.Application.Validators;

public sealed class ApiKeyAuthOptionsValidator : AbstractValidator<ApiKeyAuthOptions>
{
    public ApiKeyAuthOptionsValidator()
    {
        RuleFor(x => x.ApiKeys)
            .NotEmpty().WithMessage("APIKEYS is required");

        // TODO: Check every permission if exists (read, write, create etc.)
    }
}
