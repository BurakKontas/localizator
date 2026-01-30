using Localizator.Shared.Base;
using Localizator.Shared.Resources;
using System.Text.RegularExpressions;

namespace Localizator.Namespace.Domain.Namespace.ValueObjects;

public sealed class SupportedLanguage(string value) : BaseValueObject<string>(Validate(value))
{
    private SupportedLanguage() : this(string.Empty) { } // EF

    public string LanguageCode { get; set; } = value;

    public static readonly int LENGTH = 2;

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(Errors.InvalidLanguageCode, nameof(value));

        if (!Regex.IsMatch(value, "^[a-z]{2}$"))
            throw new ArgumentException(Errors.InvalidLanguageCode, nameof(value));

        return value.ToLowerInvariant();
    }

    public static implicit operator string(SupportedLanguage language) => language.Value;
    public static explicit operator SupportedLanguage(string value) => new(value);

    public override string ToString() => Value;
}