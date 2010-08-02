using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;

namespace Rocker.Rest
{
    public interface IConnectionDetails
    {
        Uri ToUri();
        ICredentials GetCredentials();
        string ToUriString(bool includeAuth);

        string Database { get; set; }
    }

    public class ConnectionDetails : IConnectionDetails
    {

        private const string _protocol = "http";
        private const string _host = "localhost";
        private const int _port = 5984;

        public string Protocol { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Uri ToUri()
        {
            return new Uri(this.ToUriString(false));
        }

        public ICredentials GetCredentials()
        {
            if (string.IsNullOrEmpty(UserName))
                return null;
            return new NetworkCredential(UserName, Password);
        }

        public string ToUriString(bool includeAuth)
        {
            if (includeAuth && !string.IsNullOrEmpty(UserName))
                return string.Concat(Protocol, "://", UserName, ":", Password, "@", Host, ":", Port);
            else
                return string.Concat(Protocol, "://", Host, ":", Port);
        }

        public ConnectionDetails(string connectionString)
        {
            var localconstring = connectionString;
            var connobject = ConfigurationManager.ConnectionStrings[connectionString];

            if (connobject != null && !string.IsNullOrEmpty(connobject.ConnectionString))
            {
                localconstring = connobject.ConnectionString;
            }

            var vals = localconstring.ToDictionary(true);

            Protocol = _protocol;
            if (vals.ContainsKey("protcol"))
                Protocol = vals["protcol"];

            Host = _host;
            if (vals.ContainsKey("host"))
                Host = vals["host"];

            var port = _port;
            if (vals.ContainsKey("port"))
            {
                if (!int.TryParse(vals["port"], out port))
                    port = _port;
            }
            Port = port;

            UserName = null;
            if (vals.ContainsKey("username"))
                UserName = vals["username"];

            Password = "";
            if (vals.ContainsKey("password"))
                Password = vals["password"];

            Database = vals["database"];
        }
    }
}
