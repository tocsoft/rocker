using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lounger
{
    public static class UriExtentions
    {
        public static Uri AppendPart(this Uri url, string part)
        {
            string path = url.ToString();

            if (part.StartsWith("/"))
                path += part;
            else
                path = string.Concat(path, "/", part);

            return new Uri(path);
        }
    }
}
