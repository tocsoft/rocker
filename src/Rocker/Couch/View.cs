using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{
    public class View<TKey, TValue>
    {
        public int total_rows { get; set; }
        public int offset { get; set; }
        public Row<TKey, TValue>[] rows { get; set; }
    }

    public class MultiView<TKey>
    {
        public int total_rows { get; set; }
        public int offset { get; set; }
        public Row<TKey, object>[] rows { get; set; }
    }

    public class Row<TKey, TValue>
    {
        public string Id { get; set; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public TValue Doc { get; set; }
    }
}
