using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Json;
using Rocker.Rest;

namespace Rocker.ElasticSearch
{
    public class ElasticIndex
    {
        readonly ISerializer _serializer;
        readonly IRestClient _client;
        readonly string _index;
        readonly string _type;

        public ElasticIndex(IRestClient client, ISerializer serializer, string indexName, string typeName)
        {
            _serializer = serializer;
            _client = client;
            _index = indexName;
            _type = typeName;
        }
        public void UpdateMapping<T>(ElasticDataClassMapping<T> mapping)
        {
            var res = _client.DoRequest("_mapping?ignore_conflicts=true", "PUT", _serializer.Serialize(mapping.GetMappingObject()), "application/json");
        }
        public void UpdateMapping<T>(Func<ElasticDataClassMapping<T>,ElasticDataClassMapping<T>> mapping)
        {
            UpdateMapping<T>(mapping.Invoke(new ElasticDataClassMapping<T>(_type)));
            //_client.DoRequest(id, "PUT", _serializer.Serialize(obj), "application/json");
        }
        public void Add(object obj, string id)
        {
            _client.DoRequest(id, "PUT", _serializer.Serialize(obj), "application/json");
        }

        public void Update(object obj, string id)
        {
            Delete(id);
            Add(obj, id);
        }
        public void Delete(string id)
        {
            _client.DoRequest(id, "DELETE");
        }

        public ElasticResults<T> Search<T>(Func<ElasticSearchQuery, ElasticSearchQuery> query)
        {
            return Search<T>(query.Invoke(new ElasticSearchQuery()));
        }
        public ElasticResults<T> Search<T>(ElasticSearchQuery query)
        {
            string jsonQuery = _serializer.Serialize(query.GenerateQueryObject());
            var res = _client.DoRequest("_search", "POST", jsonQuery, "application/json");

            return _serializer.Deserialize<ElasticResults<T>>(res);

        }

    }
}
