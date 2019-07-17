using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mecha.Helpers
{
    internal static class Utils
    {
        public static IEnumerable<string> NameToDisplay(string value)
        {
            var fullWords = Regex.Matches(value, "[A-Z][a-z]+");

            if (fullWords.Count == 0)
                yield return value;

            int i = 0;
            foreach (Match x in fullWords)
            {
                int tmp = i;
                i = x.Index + x.Length;
                if (tmp < x.Index)
                    yield return value.Substring(tmp, x.Index - tmp);
                yield return x.Value;
            }
        }

        public static string NameToDisplayName(string value)
        {
            return string.Join(" ", NameToDisplay(value));
        }

        public static string TrimEnd(this string s, string value)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s));

            return s.EndsWith(value, StringComparison.CurrentCulture)
                ? s.Remove(s.Length - value.Length)
                : s;
        }
        
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsPrimitive && type != typeof(bool) && type != typeof(char);
        }
    }
}
