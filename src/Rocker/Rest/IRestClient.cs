using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rocker.Rest
{
    public interface IRestClient
    {
        IRestClient SubClient(string path);

        string DoRequest(string query, string method, Action<Stream> data, string contentType);
        string DoRequest(string query, string method, string data, string contentType);
        string DoRequest(string query, string method);
        Stream DoDataRequest(string query, string method);
        Stream DoDataRequest(string query, string method, Action<Stream> data, string contentType);
    }
}
