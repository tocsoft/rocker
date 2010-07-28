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

        private static CouchServer ConnectToServer(string protocol, string host, int port)
        {
            var rest = new RestClient(new Uri(protocol + "://" + host + ":" + port.ToString()));
            return new CouchServer(rest, new JsonSerializer());
        }

        private static CouchServer ConnectToServer(string protocol, string host, int port, string username, string password)
        {
            var rest = new RestClient(new Uri(protocol + "://" + host + ":" + port.ToString()), new NetworkCredential(username, password));
            return new CouchServer(rest, new JsonSerializer());
        }

        
        
        public static CouchDatabase ConnectToDatabase(string connectionString)
        {
            var conDetails = new ConnectionDetails(connectionString);

            if (string.IsNullOrEmpty(conDetails.UserName))
                return ConnectToServer(conDetails.Protocol, conDetails.Host, conDetails.Port).ConnectToDatabase(conDetails.Database);
            else
                return ConnectToServer(conDetails.Protocol, conDetails.Host, conDetails.Port, conDetails.UserName, conDetails.Password).ConnectToDatabase(conDetails.Database);
        }



        #region Replication

        public static void Replicate(
            string hostServerConnectionString,
            string fromServerConnectionString,
            string toServerConnectionString,
            string filter,
            object filterquery)
        {
            var host = new ConnectionDetails(hostServerConnectionString);
            var from = new ConnectionDetails(fromServerConnectionString);
            var to = new ConnectionDetails(toServerConnectionString);

            RestClient rest;
            if (!string.IsNullOrEmpty(host.UserName))
                rest = new RestClient(host.ToUri(), new NetworkCredential(host.UserName, host.Password));
            else
                rest = new RestClient(host.ToUri());

            var serializer = new JsonSerializer();
            if (string.IsNullOrEmpty(filter))
                rest.DoRequest("_replicate", "POST", serializer.Serialize(new
                {
                    source = string.Concat(from.ToUriString(true),  "/", from.Database),
                    target = string.Concat(to.ToUriString(true), "/", to.Database),
                }), "application/json");
            else
                rest.DoRequest("_replicate", "POST", serializer.Serialize(new
                {
                    source = string.Concat(from.ToUriString(true), "/", from.Database),
                    target = string.Concat(to.ToUriString(true), "/", to.Database),
                    filter = filter,
                    query_params = filterquery
                }), "application/json");
        }

        #endregion

        private class ConnectionDetails
        {

            private const string _protocol = "http";
            private const string _host = "localhost";
            private const int _port = 5984;

            public string Protocol { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public string Database { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            public Uri ToUri()
            {
                return new Uri(this.ToUriString(false));
            }
            public string ToUriString(bool includeAuth)
            {
                if (includeAuth && !string.IsNullOrEmpty(UserName))
                    return string.Concat(Protocol , "://", UserName, ":", Password, "@" , Host , ":" , Port);
                else
                    return string.Concat(Protocol, "://", Host, ":", Port);
            }
            public ConnectionDetails(string connectionString)
            {
                var localconstring = connectionString;
                var connobject = ConfigurationManager.ConnectionStrings[connectionString];

                if (connobject != null && !string.IsNullOrEmpty(connobject.ConnectionString))
                {
                    localconstring = connobject.ConnectionString;
                }

                var vals = localconstring.ToDictionary(true);

                Protocol = _protocol;
                if (vals.ContainsKey("protcol"))
                    Protocol = vals["protcol"];

                Host = _host;
                if (vals.ContainsKey("host"))
                    Host = vals["host"];

                var port = _port;
                if (vals.ContainsKey("port"))
                {
                    if (!int.TryParse(vals["port"], out port))
                        port = _port;
                }
                Port = port;

                UserName = null;
                if (vals.ContainsKey("username"))
                    UserName = vals["username"];

                Password = "";
                if (vals.ContainsKey("password"))
                    Password = vals["password"];

                Database = vals["database"];
            }
        }

    }
}
