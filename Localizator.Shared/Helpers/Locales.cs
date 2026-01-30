namespace Localizator.Shared.Helpers;

public static class Locales
{
    public const string English = "en-US";
    public const string Turkish = "tr-TR";

    public const string Default = English;

    public static readonly IReadOnlyCollection<string> All =
    [
        English,
        Turkish
    ];
}