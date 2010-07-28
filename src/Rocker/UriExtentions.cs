using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker
{
    public static class UriExtentions
    {
        public static Uri AppendPart(this Uri url, string part)
        {
            string path = url.ToString();
            bool start  = part.StartsWith("/");
            bool end = path.EndsWith("/");

            if (start || end)
                path += part;
            else if (start && end)
                path += part.Substring(1); //trim off the leading '/'
            else
                path = string.Concat(path, "/", part);

            return new Uri(path);
        }
    }
}
