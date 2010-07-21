using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker
{
    internal static class StringExtentions
    {
        internal static IDictionary<string, string> ToDictionary(this string str)
        {
            return ToDictionary(str, false);
        }
        internal static IDictionary<string, string> ToDictionary(this string str, bool lowerKeys)
        {
            return ToDictionary(str, ';', '=', lowerKeys);
        }

        internal static IDictionary<string, string> ToDictionary(this string str, char rowSeperator, char keyValueSeperator, bool lowerKeys)
        {
            var rows = str.Split(new char[] { rowSeperator }, StringSplitOptions.RemoveEmptyEntries);

            return rows
                .Select(x => x.Split(keyValueSeperator))
                .Aggregate(
                        new Dictionary<string, string>(), 
                        (dic, s) =>
                        {
                            if (s.Length > 1)
                            {
                                var key = s[0];
                                if (lowerKeys)
                                    key = key.ToLower();
                                dic.Add(key, s[1]);
                            }
                            return dic;
                        }
                    );
        }
    }
}
