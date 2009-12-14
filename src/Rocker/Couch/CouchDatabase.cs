using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;
using Rocker.Json;
using System.Net;
using System.IO;
using Rocker.Couch.Exceptions;

namespace Rocker.Couch
{
    public sealed class CouchDatabase
    {
        readonly IRestClient _client;
        readonly ISerializer _serializer;
        Dictionary<object, RevisionInfo> _info = new Dictionary<object, RevisionInfo>();
        public CouchDatabase(IRestClient client, ISerializer serializer)
        {
            _serializer = serializer;
            _client = client;
        }


        public RevisionInfo SaveDocument<T>(T item)
        {
            RevisionInfo info = GetInfo(item);
            
            string id = null;
            string rev = null;
            if (info == null)
                id = typeof(T).FullName + "$" + Guid.NewGuid();
            else
            {
                id = info._id;
                rev = info._rev;
            } 
            return SaveDocument(item, id, rev);
        }

        public RevisionInfo SaveDocument<T>(T item, string id)
        {
            return SaveDocument(item, id, null);
        }

        public RevisionInfo SaveDocument<T>(T item, string id, string rev)
        {
            RevisionInfo info = null;
            try
            {
                string json = _serializer.Serialize(item);
                if (!string.IsNullOrEmpty(rev))
                    json = string.Concat("{ ", "\"_rev\":\"", rev,"\",", json.Substring(1));
                string ret = _client.DoRequest(id, "PUT", json , "application/json");
                info = _serializer.Deserialize<DocumentInfo>(ret).Convert();
                UpdateInfoStore(item, info);
            }
            catch(Rest.RestException ex)
            {
                throw For(ex);
            }

            return info;
        }

        public T GetDocument<T>(string id)
        {
            return GetDocument<T>(id, null);
        }
        public T GetDocument<T>(RevisionInfo info)
        {
            return GetDocument<T>(info._id, info._rev);
        }
        public T GetDocument<T>(string id, string revision)
        {
            try
            {
                string ret = _client.DoRequest(EncodeId(id), "GET");
                RevisionInfo info = _serializer.Deserialize<RevisionInfo>(ret);

                T result = _serializer.Deserialize<T>(ret);
                UpdateInfoStore(result, info);
                return result;
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }

        public Stream GetAttachment<T>(T item, string path)
        {
            RevisionInfo info = GetInfo(item);
            return GetAttachment(info, path);
        }
        public Stream GetAttachment(RevisionInfo info, string path)
        {
            return GetAttachment(info._id, path);
        }
        public Stream GetAttachment(string id, string path)
        {
            try
            {
                Action<Stream> a = null;
                return _client.DoDataRequest(string.Format("{0}/{1}", EncodeId(id), path), "GET");
            }

            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }


        public RevisionInfo SaveAttachment<T>(T item, string path, string contentType, Stream data)
        {
            var info = GetInfo(item);
            return SaveAttachment(info, path, contentType, data);
        }
        public RevisionInfo SaveAttachment(RevisionInfo info, string path, string contentType, Stream data)
        {
            return SaveAttachment(info._id, info._rev, path, contentType, data);
        }
        public RevisionInfo SaveAttachment(string id, string revision, string path, string contentType, Stream data)
        {
            try
            {
                Action<Stream> a = s => CopyStream(data, s);
                return _serializer.Deserialize<DocumentInfo>(
                    _client.DoRequest(string.Format("{0}/{1}?rev={2}", EncodeId(id), path, revision), "PUT", a, contentType)
                    ).Convert();
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }

        public View<TKey, TValue> GetView<TKey, TValue>(string document, string view)
        {
            try
            {
                string ret = _client.DoRequest(string.Format("_design/{0}/_view/{1}", EncodeId(document), EncodeId(view)), "GET");
                View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
                View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);

                for (int i = 0; i < infoView.total_rows; i++)
                {
                    UpdateInfoStore(results.rows[i].Value, infoView.rows[i].Value);
                }

                return results;
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }

        public View<TKey, TValue> GetTempView<TKey, TValue>(string query)
        {
            try
            {
                string ret = _client.DoRequest("_temp_view", "POST", _serializer.Serialize(new ViewMap(query)), "application/json");
                View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
                View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);

                if (infoView.rows.Length > 0 && infoView.rows[0].Value._id != null)
                {
                    for (int i = 0; i < infoView.total_rows; i++)
                    {
                        UpdateInfoStore(results.rows[i].Value, infoView.rows[i].Value);
                    }
                }
                return results;
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }


        public RevisionInfo GetInfo(object item)
        {
            if (_info.ContainsKey(item))
                return _info[item];
            else
                return null;
        }

        public DatabaseInfo GetInfo()
        {
            try
            {
                var r = _client.DoRequest("", "GET");
                return _serializer.Deserialize<DatabaseInfo>(r);
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }

        private void UpdateInfoStore(object item, RevisionInfo info)
        {
            if (_info.ContainsKey(item))
                _info[item] = info;
            else
                _info.Add(item, info);
        }
        

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }

        private static CouchException For(Rest.RestException restException)
        {
            //TODO encpsulate all the status code/description that are returned by couch into descriptive exceptions!!!
            return new CouchException(restException);
        }

        private static string EncodeId(string id)
        {
            return System.Web.HttpUtility.UrlEncode(id);
        }

    }
}
