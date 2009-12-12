using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Lounger.Rest
{
    public class RestClient : IRestClient
    {
        public RestClient(Uri url)
        {
            Url = url;
        }
        public RestClient(string url) : this(new Uri(url))
        {
        }
        public Uri Url { get; set; }


        public IRestClient SubClient(string path)
        {
            return new RestClient(new Uri(Url, path));
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


            Stream stream = DoRequest(query, method, a, contenttype);
            System.IO.StreamReader sr = new StreamReader(stream);

            return sr.ReadToEnd();
        }

        public Stream DoRequest(string query, string method, Action<Stream> data, string contenttype)
        {
            HttpWebRequest req = WebRequest.Create(Url.AppendPart(query)) as HttpWebRequest;
            req.Method = method;
            
            //req.Timeout = System.Threading.Timeout.Infinite;
            if (!string.IsNullOrEmpty(contenttype ))
                req.ContentType = contenttype;

            if (data != null)
            {
                using (Stream ps = req.GetRequestStream())
                {
                    data.Invoke(ps);
                }
            }

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            return resp.GetResponseStream();
        }     
    }
}
