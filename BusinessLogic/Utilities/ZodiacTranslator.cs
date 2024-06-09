using DataAccess.Enums;
using DataAccess.Models;

namespace BusinessLogic.Utilities;

public static class ZodiacTranslator
{
    private static readonly Dictionary<string, string> UkrainianToEnglish = new()
    {
        { "овен", "aries" },
        { "телець", "taurus" },
        { "близнюки", "gemini" },
        { "рак", "cancer" },
        { "лев", "leo" },
        { "діва", "virgo" },
        { "терези", "libra" },
        { "скорпіон", "scorpio" },
        { "стрілець", "sagittarius" },
        { "козоріг", "capricorn" },
        { "водолій", "aquarius" },
        { "риби", "pisces" }
    };

    private static readonly Dictionary<string, string> EnglishToUkrainian = new();
    public static string UkrainianSigns => string.Join("\n", UkrainianToEnglish.Keys.Select(Capitalize));

    static ZodiacTranslator()
    {
        foreach (var pair in UkrainianToEnglish)
        {
            EnglishToUkrainian[pair.Value] = pair.Key;
        }
    }

    public static string? TranslateUkrainianToEnglish(this string ukrainianSign)
    {
        return UkrainianToEnglish.GetValueOrDefault(ukrainianSign.ToLower())?.Capitalize();
    }

    public static string? TranslateEnglishToUkrainian(this string englishSign)
    {
        return EnglishToUkrainian.GetValueOrDefault(englishSign.ToLower())?.Capitalize();
    }

    public static string TranslateEnglishToUkrainian(this IEnumerable<string> englishSigns)
    {
        return string.Join(", ", englishSigns.Select(TranslateEnglishToUkrainian));
    }

    public static string? TranslateMoonPhaseToUkrainian(this MoonPhase moonPhase)
    {
        return moonPhase switch
        {
            MoonPhase.NewMoon => "Молодик",
            MoonPhase.WaxingCrescent or MoonPhase.WaxingGibbous => "Зростаючий Місяць",
            MoonPhase.FirstQuarter => "Перша Чверть",
            MoonPhase.FullMoon => "Повня",
            MoonPhase.WaningGibbous or MoonPhase.WaningCrescent => "Спадаючий Місяць",
            MoonPhase.LastQuarter => "Третя Чверть",
            _ => null
        };
    }

    public static string? TranslatePeriodToUkrainian(this Period period)
    {
        return period switch
        {
            Period.Daily => "день",
            Period.Weekly => "тиждень",
            Period.Monthly => "місяць",
            _ => null
        };
    }

    private static string Capitalize(this string str)
    {
        return str[..1].ToUpper() + str[1..];
    }
}
