using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lounger.Json
{
    public interface ISerializer
    {
        T Deserialize<T>(string s);
        string Serialize(object obj);
    }
}
