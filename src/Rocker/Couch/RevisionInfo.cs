using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{

    public class RevisionInfo
    {
        public RevisionInfo()
        {
            _attachments = new Dictionary<string, FileDetails>();
        }
        public string _id { get; set; }
        public string _rev { get; set; }
        public bool _deleted { get; set; }
        public Dictionary<string, FileDetails> _attachments { get; set; }

        public class FileDetails
        {
            public string content_type { get; set; }
            public long revpos { get; set; }
            public long length { get; set; }
            public bool stub { get; set; }
        }
        
        public DocumentInfo convert()
        {

            return new DocumentInfo() { Id = _id, Rev = _rev, Ok = true };
        }

    }
    public class RawDataResults : Dictionary<string, string>
    { 
    
    }
}
