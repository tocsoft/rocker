using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Rocker.Rest
{
    public class RestException : Exception
    {
        public HttpStatusCode Status { get; private set; }
        public string StatusDescription { get; private set; }

        public RestException(HttpStatusCode status, Exception innerException)
            : base(string.Format("Server returned {0}", status), innerException)
        {
            Status = status;
        }
        public RestException(HttpStatusCode status)
            : this(status, (Exception)null)
        {
        }

        public RestException(HttpStatusCode status, string statusDescription, Exception innerException)
            : base(string.Format("Server returned {0} - \"{1}\"", status, statusDescription), innerException)
        {
            Status = status;
            StatusDescription = statusDescription;
        }
        public RestException(HttpStatusCode status,  string statusDescription)
            : this(status, statusDescription, (Exception)null)
        {
        }
    }
}
