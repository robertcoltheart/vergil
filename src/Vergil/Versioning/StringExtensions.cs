using System.Text.RegularExpressions;

namespace Vergil.Versioning;

public static class StringExtensions
{
    public static string Escaped(this string value)
    {
        return Regex.Replace(value, "[^a-zA-Z0-9-_]", "-");
    }
}
