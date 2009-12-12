using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lounger.Rest;
using Lounger.Json;
using System.Net;
using System.IO;

namespace Lounger.Couch
{
    public sealed class CouchDatabase
    {
        readonly IRestClient _client;
        readonly ISerializer _serializer;
        Dictionary<object, DocumentInfo> _info = new Dictionary<object, DocumentInfo>();
        public CouchDatabase(IRestClient client, ISerializer serializer)
        {
            _serializer = serializer;
            _client = client;
        }


        public DocumentInfo SaveDocument<T>(T item)
        {
            DocumentInfo info = GetInfo(item);
            
            string id = null;
            string rev = null;
            if (info == null)
                id = typeof(T).FullName + "$" + Guid.NewGuid();
            else
            {
                id = info.Id;
                rev = info.Rev;
            } 
            return SaveDocument(item, id, rev);
        }

        public DocumentInfo SaveDocument<T>(T item, string id)
        {
            return SaveDocument(item, id, null);
        }

        public DocumentInfo SaveDocument<T>(T item, string id, string rev)
        {
            DocumentInfo info = null;
            try
            {
                string json = _serializer.Serialize(item);
                if (!string.IsNullOrEmpty(rev))
                    json = string.Concat("{ ", "\"_rev\":\"", rev,"\",", json.Substring(1));
                string ret = _client.DoRequest(id, "PUT", json , "application/json");
                info = _serializer.Deserialize<DocumentInfo>(ret);
                UpdateInfoStore(item, info);
            }
            catch
            {
            }

            return info;
        }

        public T GetDocument<T>(string id)
        {
            return GetDocument<T>(id, null);
        }
        
        private class RevisionInfo
        {
            public string _id { get; set; }
            public string _rev { get; set; }

            public DocumentInfo convert()
            {

                return new DocumentInfo() { Id = _id, Rev = _rev, Ok = true };
            }
            
        }

        public T GetDocument<T>(string id, string revision)
        {
            string ret = _client.DoRequest(id, "GET");
            RevisionInfo info = _serializer.Deserialize<RevisionInfo>(ret);
            DocumentInfo di = new DocumentInfo() { Id = info._id, Rev = info._rev, Ok = true };
            T result = _serializer.Deserialize<T>(ret);
            UpdateInfoStore(result, di);
            return result;
        }


        public View<TKey, TValue> GetView<TKey, TValue>(string document, string view)
        {
            string ret = _client.DoRequest(string.Format("_design/{0}/_view/{1}", document, view), "GET");
            View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
            View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);

            for (int i = 0; i < infoView.total_rows; i++)
            {
                UpdateInfoStore(results.rows[i].Value, infoView.rows[i].Value.convert());
            }

            return results;
        }

        public View<TKey, TValue> GetTempView<TKey, TValue>(string query)
        {
            string ret = _client.DoRequest("_temp_view", "POST", _serializer.Serialize(new ViewMap(query)), "application/json");
            View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
            View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);

            if (infoView.rows.Length > 0 && infoView.rows[0].Value._id != null)
            {
                for (int i = 0; i < infoView.total_rows; i++)
                {
                    UpdateInfoStore(results.rows[i].Value, infoView.rows[i].Value.convert());
                }
            }
            return results;
        }

        public T GetDocument<T>(DocumentInfo info)
        {
            return GetDocument<T>(info.Id, info.Rev);
        }

        private void UpdateInfoStore(object item, DocumentInfo info)
        {
            if (_info.ContainsKey(item))
                _info[item] = info;
            else
                _info.Add(item, info);
        }

        public DocumentInfo GetInfo(object item)
        {
            if (_info.ContainsKey(item))
                return _info[item];
            else
                return null;
        }

        public DatabaseInfo GetInfo()
        {
            var r = _client.DoRequest("", "GET");
            return _serializer.Deserialize<DatabaseInfo>(r);
        }         
    }
}
