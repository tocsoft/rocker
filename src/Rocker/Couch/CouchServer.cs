using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;
using Rocker.Json;

namespace Rocker.Couch
{
    public sealed class CouchServer
    {
        readonly ISerializer _serializer;
        readonly IRestClient _client;
        readonly IConnectionDetails _connection;

        public CouchServer(IRestClient client, ISerializer serializer, IConnectionDetails connection)
        {
            _serializer = serializer;
            _client = client;
            _connection = connection;
        }

        public string[] GetDatabaseNames()
        {
            return _serializer.Deserialize<string[]>(_client.DoRequest("_all_dbs", "GET"));
        }


        public void CreateDatabase(string name)
        {
            _client.DoRequest(name + "/", "PUT");

        }
        public CouchDatabase ConnectToDatabase(string databaseName)
        {
            var server = _client.SubClient(databaseName);
            return new CouchDatabase(server, _serializer);
        }

        public void Replicate(
           IConnectionDetails from,
           IConnectionDetails to,
           string filter,
           object filterquery)
        {
            if (string.IsNullOrEmpty(filter))
                _client.DoRequest("_replicate", "POST", _serializer.Serialize(new
                {
                    source = string.Concat(from.ToUriString(true), "/", from["database"]),
                    target = string.Concat(to.ToUriString(true), "/", to["database"])
                }), "application/json");
            else
                _client.DoRequest("_replicate", "POST", _serializer.Serialize(new
                {
                    source = string.Concat(from.ToUriString(true), "/", from["database"]),
                    target = string.Concat(to.ToUriString(true), "/", to["database"]),
                    filter = filter,
                    query_params = filterquery
                }), "application/json");
        }
        public void Replicate(
           IConnectionDetails from,
           IConnectionDetails to)
        {
            Replicate(from, to, null, null);
        }
        public void ReplicateTo(
           IConnectionDetails destination,
           string filter,
           object filterquery)
        {
            Replicate(_connection, destination, filter, filterquery);
        }
        public void ReplicateFrom(
           IConnectionDetails source,
           string filter,
           object filterquery)
        {
            Replicate(source, _connection, filter, filterquery);
        }
        public void ReplicateTo(
           IConnectionDetails destination)
        {
            Replicate(_connection, destination, null, null);
        }
        public void ReplicateFrom(
           IConnectionDetails source)
        {
            Replicate(source, _connection, null, null);
        }

    }
}
