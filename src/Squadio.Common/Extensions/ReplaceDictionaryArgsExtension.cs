using System.Collections.Generic;

namespace Squadio.Common.Extensions
{
    public static class ReplaceDictionaryArgsExtension
    {
        public static string Replace(this string str, IDictionary<string, string> args)
        {
            if (args != null && args.Count > 0)
            {
                foreach ((string str2, string newValue2) in (IEnumerable<KeyValuePair<string, string>>) args)
                    str = str.Replace(str2, newValue2);
            }
            return str;
        }
    }
}