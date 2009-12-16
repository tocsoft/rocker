using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{
    public class ViewQuery
    {
        private string _key;
        private string _endKey;
        private string _name;
        private string _view;
        private int? _take;
        private int? _skip;
        private bool _descending;
        public ViewQuery(string name, string view)
        {
            _name = name;
            _view = view;
            _descending = false;
        }

        public ViewQuery Key(string key)
        {
            _key = key;
            return this;
        }

        public ViewQuery EndKey(string key)
        {
            _endKey = key;
            return this;
        }

        public ViewQuery Take(int count)
        {
            _take = count;
            return this;
        }

        public ViewQuery Skip(int count)
        {
            _skip = count;
            return this;
        }
        
        public ViewQuery OrderDescending()
        {
            _descending = true;
            return this;
        }


        public string GenerateQuery()
        {
            string q = string.Format("_design/{0}/_view/{0}", _name, _view);
            if (!string.IsNullOrEmpty(_key))
            {
                if (string.IsNullOrEmpty(_endKey))
                {
                    q = AddQueryStirng(q, "key", _key);
                }
                else
                {

                    q = AddQueryStirng(q, "startkey", _key);

                    q = AddQueryStirng(q, "endkey", _endKey);
                }
            }

            if (_take.HasValue)
                q = AddQueryStirng(q, "limit", _take.Value);

            if (_skip.HasValue)
                q = AddQueryStirng(q, "skip", _skip.Value);
            if (_descending)
                q = AddQueryStirng(q, "descending", true);

            return q;
        }

        public string AddQueryStirng(string qs, string name, object value)
        {
            if (qs.Contains("?"))
                return string.Concat(qs, "&", name, "=", value);
            else
                return string.Concat(qs, "?", name, "=", value);
        }
    }
}
