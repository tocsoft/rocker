using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rocker.Json
{
    public class JsonSerializer : ISerializer
    {
        #region ISerializer Members

        public T Deserialize<T>(string s)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        #endregion
    }
}
