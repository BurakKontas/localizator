using Localizator.Shared.Helpers;
using System.Globalization;

namespace Localizator.Shared.Providers;

public static class LocaleProvider
{
    public static bool IsSupported(string? locale)
            => !string.IsNullOrWhiteSpace(locale)
               && Locales.All.Contains(locale);

    public static string Normalize(string? locale)
    {
        if (IsSupported(locale))
            return locale!;

        return Locales.Default;
    }

    public static CultureInfo GetCulture(string locale) => new(locale);
}