using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lounger.Rest;
using Lounger.Json;

namespace Lounger.Couch
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

        public static CouchServer ConnectToServer(string host)
        {
            return ConnectToServer(_protocol, host, _port);
        }
        public static CouchServer ConnectToServer()
        {
            return ConnectToServer(_protocol, _host, _port);
        }

        public static CouchServer ConnectToServer(string host, int port)
        {
            return ConnectToServer(_protocol, host, port);
        }

        public static CouchDatabase ConnectToDatabase(string host, string database)
        {
            return ConnectToServer(_protocol, host, _port).ConnectToDatabase(database);
        }
        public static CouchDatabase ConnectToDatabase(string database)
        {
            return ConnectToServer(_protocol, _host, _port).ConnectToDatabase(database);
        }

        public static CouchDatabase ConnectToDatabase(string host, int port, string database)
        {
            return ConnectToServer(_protocol, host, port).ConnectToDatabase(database);
        }


    }
}
