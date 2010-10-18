using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Rocker.Rest
{
    public class RestClient : IRestClient
    {

        public RestClient(IConnectionDetails connectionDetails) : this(connectionDetails.ToUri(), connectionDetails.GetCredentials())
        { 
        }
        public RestClient(Uri url, ICredentials credentials)
        {
            Url = url;
            Credentials = credentials;
        }
        public RestClient(Uri url) : this(url, null)
        {
        }
        public RestClient(string url)
            : this(url, null)
        {
        }
        public RestClient(string url, ICredentials credentials)
            : this(new Uri(url), credentials)
        {
        }

        public Uri Url { get; set; }
        public ICredentials Credentials { get; set; }


        public IRestClient SubClient(string path)
        {
            return new RestClient(Url.AppendPart(path));
        }

        public string DoRequest(string query, string method)
        {
            return DoRequest(query, method, "", null);
        }
        public string DoRequest(string query, string method, string data, string contenttype)
        {
            Action<Stream> a = null;
            if (!string.IsNullOrEmpty(data))
                a = (s => (new BinaryWriter(s)).Write(Encoding.UTF8.GetBytes(data)));


            return DoRequest(query, method, a, contenttype);

        }
        public string DoRequest(string query, string method, Action<Stream> data, string contenttype)
        {

            Stream stream = DoDataRequest(query, method, data, contenttype);
            System.IO.StreamReader sr = new StreamReader(stream, Encoding.UTF8);

            return sr.ReadToEnd();
        }

        public Stream DoDataRequest(string query, string method)
        {
            Action<Stream> a = null;
            return DoDataRequest(query, method, a, null);
        }
        public Stream DoDataRequest(string query, string method, Action<Stream> data, string contenttype)
        {
            HttpWebRequest req = WebRequest.Create(Url.AppendPart(query)) as HttpWebRequest;
            
            if (Credentials != null)
                req.Credentials = Credentials;
            
            req.Method = method;

            //req.Timeout = System.Threading.Timeout.Infinite;
            if (!string.IsNullOrEmpty(contenttype))
                req.ContentType = contenttype;

            if (data != null)
            {
                using (Stream ps = req.GetRequestStream())
                {
                    data.Invoke(ps);
                }
            }
            try
            {
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Created)
                    return resp.GetResponseStream();

                throw new RestException(resp.StatusCode, resp.StatusDescription);
            }
            catch (WebException ex)
            {
                HttpWebResponse resp = ex.Response as HttpWebResponse;
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string contents = sr.ReadToEnd();
                throw new RestException(resp.StatusCode, resp.StatusDescription);
            }
            
            
        }

    }
}
