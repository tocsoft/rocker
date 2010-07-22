using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;
using Rocker.Json;
using Rocker;
using System.Configuration;

namespace Rocker.Couch
{
    public static class CouchFactory
    {
        private const string _protocol = "http";
        private const string _host = "localhost";
        private const int _port = 5984;

        public static CouchServer ConnectToServer(string protocol, string host, int port)
        {
            var rest = new RestClient(new Uri(protocol + "://" + host + ":" + port.ToString()));
            return new CouchServer(rest, new JsonSerializer());
        }

        public static CouchServer ConnectToServer()
        {
            return ConnectToServer("");
        }

        public static CouchServer ConnectToServer(string connectionString)
        {
            var vals = connectionString.ToDictionary(true);


            var protocol = _protocol;
            if (vals.ContainsKey("protcol"))
                protocol = vals["protcol"];

            var host = _host;
            if (vals.ContainsKey("host"))
                host = vals["host"];

            var port = _port;
            if (vals.ContainsKey("port"))
            {
                if(!int.TryParse(vals["port"], out port))
                    port = _port;
            }

           // var database = vals["database"];

            return ConnectToServer(protocol, host, port);
        }

        public static CouchDatabase ConnectToDatabase(string connectionString)
        {
            var localconstring = connectionString;
            var constrings = ConfigurationManager.ConnectionStrings[connectionString];
            if (!(constrings != null && !string.IsNullOrEmpty(constrings.ConnectionString)))
            {
                localconstring = constrings.ConnectionString;
            }

            var vals = localconstring.ToDictionary(true);

            var protocol = _protocol;
            if (vals.ContainsKey("protcol"))
                protocol = vals["protcol"];

            var host = _host;
            if (vals.ContainsKey("host"))
                host = vals["host"];

            var port = _port;
            if (vals.ContainsKey("port"))
            {
                if(!int.TryParse(vals["port"], out port))
                    port = _port;
            }

            var database = vals["database"];

            return ConnectToServer(protocol, host, port).ConnectToDatabase(database);
        }


    }
}
