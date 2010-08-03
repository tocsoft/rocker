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
        CouchRevisionStore _revisionStore;
        public CouchDatabase(IRestClient client, ISerializer serializer)
        {
            _serializer = serializer;
            _client = client;
            _revisionStore = new CouchRevisionStore();
        }


        public RevisionInfo SaveDocument<T>(T item)
        {
            return SaveDocument(item, typeof(T).FullName + "~" + Guid.NewGuid());   
        }

        public RevisionInfo SaveDocument<T>(T item, string id)
        {
            RevisionInfo info = GetInfo(item);

            string lid = null;
            string rev = null;
            if (info == null)
                lid = id;
            else
            {
                lid = info._id;
                rev = info._rev;
            }
            return SaveDocument(item, lid, rev);
        }

        public RevisionInfo SaveDocument<T>(T item, string id, string rev)
        {
            RevisionInfo info = null;
            try
            {
                string json = _serializer.Serialize(item);
                if (!string.IsNullOrEmpty(rev))
                    json = string.Concat("{ \"~type\":\"", item.GetType().Name ,"\",\"_rev\":\"", rev,"\",", json.Substring(1));
                else
                    json = string.Concat("{ \"~type\":\"", item.GetType().Name, "\",", json.Substring(1));

                string ret = _client.DoRequest(id, "PUT", json, "application/json");
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

        public IEnumerable<T> GetDocuments<T>(params string[] ids)
        {
            var res = GetView<string, T>(new ViewQuery("", "").SetUrlPattern("_all_docs").IncludeDocs().Keys(ids));
            return res.rows.Select(x=>x.Doc);
        }

        public View<TKey, TValue> GetView<TKey, TValue>(string name, string view, Func<ViewQuery, ViewQuery> query)
        {
            return GetView<TKey, TValue>(query(new ViewQuery(name, view)));
        }

        public View<TKey, TValue> GetView<TKey, TValue>(string name, string view)
        {
            return GetView<TKey, TValue>(new ViewQuery(name, view));
        }
        
        public MultiView<TKey> GetView<TKey>(ViewQuery query, Func<ISerializer, TKey, string, object> map)
        {
            try
            {
                string qs = query.GenerateQuery();
                string meth = query.Method;
                string data = null;
                if (query.RequestData != null)
                    data = _serializer.Serialize(query.RequestData);

                string ret = null;
                if (data != null)
                    ret = _client.DoRequest(qs, meth, data, "application/json");
                else
                    ret = _client.DoRequest(qs, meth);
                
                View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);

                var res = _serializer.Deserialize<View<TKey, object>>(ret);
                
                List<object> rows = new List<object>();
                foreach (var r in res.rows)
                    rows.Add(map(_serializer, r.Key, r.Value.ToString()));

                MultiView<TKey> results = new MultiView<TKey>()
                {
                    offset = res.offset,
                    total_rows = res.total_rows,
                };
                for (int i = 0; i < infoView.rows.Length; i++)
                {
                    if (query.IncludingDocs)
                    {
                        UpdateInfoStore(results.rows[i].Doc, infoView.rows[i].Value);
                    }
                    UpdateInfoStore(results.rows[i].Value, infoView.rows[i].Value);
                }

                return results;
            }
            catch (Rest.RestException ex)
            {
                throw For(ex);
            }
        }

        public View<TKey, TValue> GetView<TKey, TValue>(ViewQuery query)
        {
            try
            {
                string qs = query.GenerateQuery();
                string meth = query.Method;
                string data = null;
                if (query.RequestData != null)
                    data = _serializer.Serialize(query.RequestData);

                string ret = null;
                if (data != null)
                    ret = _client.DoRequest(qs, meth, data, "application/json");
                else
                    ret = _client.DoRequest(qs, meth);

                View<TKey, RevisionInfo> infoView = null;
                try
                {
                    infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
                }
                catch { }
                View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);
                if (infoView != null)
                {
                    for (int i = 0; i < infoView.rows.Length; i++)
                    {
                        if (query.IncludingDocs)
                        {
                            UpdateInfoStore(results.rows[i].Doc, infoView.rows[i].Doc);
                        }
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

        public View<TKey, TValue> GetTempView<TKey, TValue>(string query)
        {
            try
            {
                string ret = _client.DoRequest("_temp_view", "POST", _serializer.Serialize(new ViewMap(query)), "application/json");
                View<TKey, RevisionInfo> infoView = _serializer.Deserialize<View<TKey, RevisionInfo>>(ret);
                View<TKey, TValue> results = _serializer.Deserialize<View<TKey, TValue>>(ret);

                if (infoView.rows.Length > 0 && infoView.rows[0].Value._id != null)
                {
                    for (int i = 0; i < infoView.rows.Length; i++)
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
            return _revisionStore.Lookup(item);
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
            _revisionStore.Update(item, info);
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
            if (restException.Status == HttpStatusCode.NotFound)
            {
                return new NotFoundCouchException(restException);
            }
            return new CouchException(restException);
        }

        private static string EncodeId(string id)
        {
            return System.Web.HttpUtility.UrlEncode(id);
        }

    }
}
