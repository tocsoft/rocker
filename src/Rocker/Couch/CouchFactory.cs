using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;
using Rocker.Json;
using Rocker;
using System.Configuration;
using System.Net;

namespace Rocker.Couch
{
    public static class CouchFactory
    {

        private static CouchServer ConnectToServer(ConnectionDetails conDetails)
        {

            RestClient rest = new RestClient(conDetails);

            return new CouchServer(rest, new JsonSerializer(), conDetails);
        }

        public static CouchServer ConnectToServer(string connectionString)
        {
            var conDetails = new ConnectionDetails(connectionString);

            return ConnectToServer(conDetails);
        }

        public static CouchDatabase ConnectToDatabase(string connectionString)
        {
            var conDetails = new ConnectionDetails(connectionString);

            return ConnectToServer(conDetails).ConnectToDatabase(conDetails["database"]);
        }

        public static void Replicate(this CouchServer server, string from, string to, string filter, object query)
        {
            server.Replicate(new ConnectionDetails(from), new ConnectionDetails(to), filter, query);
        }

        [Obsolete("Should use connect to server, then repliaction on server object")]
        public static void Replicate(string server, string from, string to, string filter, object query)
        {
            ConnectToServer(server).Replicate(new ConnectionDetails(from), new ConnectionDetails(to), filter, query);
        }

    }
}
