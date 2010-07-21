using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{
    public class DocumentInfo
    {
        public bool Ok { get; set; }
        public string Id { get; set; }
        public string Rev { get; set; }
        public RevisionInfo Convert()
        {
            return new RevisionInfo() { _id = Id, _rev = Rev };
        }
    }
}
