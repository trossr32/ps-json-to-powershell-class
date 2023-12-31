using System.Text.RegularExpressions;

namespace JsonToPowershellClass.Blazor.Extensions;

public static partial class UrlExtensions
{
    [GeneratedRegex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex UrlRegex();

    /// <summary>
    /// Validate URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    internal static bool IsValidUrl(this string url) => url is not null && UrlRegex().IsMatch(url);
}