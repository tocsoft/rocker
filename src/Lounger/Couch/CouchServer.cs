using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lounger.Rest;
using Lounger.Json;

namespace Lounger.Couch
{
    public sealed class CouchServer
    {
        readonly ISerializer _serializer;
        readonly IRestClient _client;

        public CouchServer(IRestClient client, ISerializer serializer)
        {
            _serializer = serializer;
            _client = client;
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
    }
}
