using System.Text;
using PluralizeService.Core;

namespace JsonToPowershellClass.Core.Helpers;

public static class StringHelper
{
    public static string ToTitleCase(this string s, bool isPlural = false)
    {
        s = isPlural ? PluralizationProvider.Singularize(s) : s;

        var sb = new StringBuilder(s.Length);
        var convertNextToUpper = true;

        foreach (var c in s)
        {
            if (!char.IsLetterOrDigit(c))
            {
                convertNextToUpper = true;

                continue;
            }

            sb.Append(convertNextToUpper ? char.ToUpper(c) : c);

            convertNextToUpper = false;
        }

        return sb.ToString();
    }
}