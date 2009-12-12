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

        }
        public string _id { get; set; }
        public string _rev { get; set; }
        public bool _deleted { get; set; }
        public DocumentInfo convert()
        {

            return new DocumentInfo() { Id = _id, Rev = _rev, Ok = true };
        }

    }
}
