using System.Text.RegularExpressions;

namespace DecayEngine.TypingsGenerator
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string s)
        {
            var x = s.Replace("_", "");
            if (x.Length == 0) return "Null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
            return char.ToLower(x[0]) + x.Substring(1);
        }
    }
}