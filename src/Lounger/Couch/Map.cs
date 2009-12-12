using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lounger.Couch
{
    public class ViewMap
    {
        public ViewMap(string q)
        {
            map = q;
        }
        public string map { get; set; } 
    }
}
