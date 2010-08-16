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
            _settings.Converters.Add(new BoolConverter());

        }

        public class BoolConverter : Newtonsoft.Json.JsonConverter
        {

            public override bool CanConvert(Type objectType)
            {
                return (typeof(bool) == objectType);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Newtonsoft.Json.JsonSerializer serializer)
            {
                return bool.Parse(reader.Value.ToString());
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if ((bool)value)
                    writer.WriteRawValue("true");
                else
                    writer.WriteRawValue("false");
            }
        }

        public T Deserialize<T>(string s)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
        }


        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, _settings);
        }

        #endregion
    }
}
