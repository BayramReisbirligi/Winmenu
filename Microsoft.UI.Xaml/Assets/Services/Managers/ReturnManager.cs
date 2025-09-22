using System.Globalization;
using Microsoft.UI;
namespace ReisProduction.Winmenu.Microsoft.UI.Xaml.Assets.Services.Managers;
internal static class ReturnManager
{
    private static readonly Random random = new();
    private static readonly SolidColorBrush
        SuccessBrush = new(Colors.DarkGreen),
        InfoBrush = new(Colors.SlateBlue),
        WarningBrush = new(Colors.DarkOrange),
        SecurityBrush = new(Colors.SlateGray);
    internal static string GetIcon(IconType iconType) =>
    iconType switch
    {
        IconType.Success => "\uE930",
        IconType.Security => "\uE83D",
        IconType.Info => "\uE946",
        IconType.Lock => "\uE72E",
        IconType.Unlock => "\uE785",
        _ => "\uE7BA"
    };
    internal static Brush GetBrush(IconType iconType) =>
    iconType switch
    {
        IconType.Success => SuccessBrush,
        IconType.Security => SecurityBrush,
        IconType.Info => InfoBrush,
        _ => WarningBrush
    };
    internal static int GetRandomInt(int min, int max) => random.Next(min, max + 1);
    #region Strings
    internal static string ForThis(this string x, string forX, TextTransform xTransform = TextTransform.None, TextTransform forXTransform = TextTransform.None)
    {
        x = x.GetString().ApplyTransform(xTransform);
        forX = forX.GetString().ApplyTransform(forXTransform);
        string language = userSettings.SelectedLanguage ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
             forKeyword = GetString("internal-For");
        return language switch
        {
            "tr" => $"{forX} {forKeyword} {x}",
            _ => $"{x} {forKeyword} {forX}"
        };
    }
    internal static string GetString(this string key) => key;
    internal static string ApplyTransform(this string input, TextTransform transform) =>
    transform switch
    {
        TextTransform.Lower => input.ToLower(),
        TextTransform.Lowercase => input.LowercaseFirstLetter(),
        TextTransform.Capitalize => input.CapitalizeFirstLetter(),
        TextTransform.Upper => input.ToUpper(),
        _ => input
    };
    internal static string LowercaseFirstLetter(this string input) => char.ToLower(input[0]) + input[1..];
    internal static string CapitalizeFirstLetter(this string input) => char.ToUpper(input[0]) + input[1..];
    #endregion
}