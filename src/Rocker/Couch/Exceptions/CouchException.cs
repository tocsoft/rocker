using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Rest;

namespace Rocker.Couch.Exceptions
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

    public class NotFoundCouchException : CouchException
    {
        public NotFoundCouchException(Exception innerException)
            : this("Document not found", innerException)
        {

        }
        public NotFoundCouchException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
