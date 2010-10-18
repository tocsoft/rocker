using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Json;
using Rocker.Rest;
using Rocker.Couch;

namespace Rocker.ElasticSearch
{
    public class ElasticServer
    {
        readonly ISerializer _serializer;
        readonly IRestClient _client;
        readonly IConnectionDetails _connection;

        public ElasticServer(IRestClient client, ISerializer serializer, IConnectionDetails connection)
        {
            _serializer = serializer;
            _client = client;
            _connection = connection;
        }
        public bool IndexExists(string indexName)
        {
            try{
                _client.SubClient(indexName).DoRequest("_status", "GET");
                return true;
            }catch{
                return false;
            }
        }

        public ElasticIndex ConnectToIndex(string indexName, string type)
        {
            var indexClient = _client.SubClient(indexName).SubClient(type);
            return new ElasticIndex(indexClient, _serializer, indexName, type);
        }

        public ElasticIndex CreateIndex(string indexName, string type)
        {
            var indexClient = _client.DoRequest(indexName, "PUT");
            return ConnectToIndex(indexName, type);
        }
        public void DeleteIndex(string indexName)
        {
            var indexClient = _client.DoRequest(indexName, "DELETE");
        }

        public void DeleteType(string indexName, string type)
        {
            var indexClient = _client.SubClient(indexName);
            indexClient.DoRequest(type, "DELETE");
        }
    }
}
