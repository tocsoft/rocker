using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lounger.Rest;

namespace Lounger.Couch.Exceptions
{
    public class CouchException : Exception
    {
        public CouchException(Exception innerException)
            : this("CouchDB error", innerException)
        {

        }
        public CouchException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
