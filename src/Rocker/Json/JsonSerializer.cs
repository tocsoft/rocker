using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rocker.Json
{
    public class JsonSerializer : ISerializer
    {
        private static Newtonsoft.Json.JsonSerializerSettings _settings;
        
        #region ISerializer Members
        static JsonSerializer()
        {
            _settings = new Newtonsoft.Json.JsonSerializerSettings();
            _settings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
            _settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        }

        public T Deserialize<T>(string s)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
        }


        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, _settings);
        }

        #endregion
    }
}
