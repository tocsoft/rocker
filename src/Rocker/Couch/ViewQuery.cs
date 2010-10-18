using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocker.Json;

namespace Rocker.Couch
{
    public class ViewQuery
    {
        private List<string> _keys;
        private string _endKey;
        private string _name;
        private string _view;
        private int? _take;
        private int? _skip;
        private bool _include_docs;
        private bool _descending;
        private string _urlpattern = "_design/{0}/_view/{1}";
        private bool _multikey = false;
        public bool IncludingDocs { get { return _include_docs; } }
        private string _groupLevel;
        private bool? _group;
        private bool? _reduce;
        private ISerializer _serializer;
        public ViewQuery(string name, string view,  ISerializer serializer)
        {
            _name = name;
            _view = view;
            _descending = false;
            _include_docs = false;
            _serializer = serializer;
        }

        public ViewQuery Key(object key)
        {
            if (_keys == null)
                _keys = new List<string>();
            else
                _multikey = true;

            _keys.Add(

                _serializer.Serialize(key)
                );
            
            return this;
        }

        public ViewQuery Keys<T>(IEnumerable<T> keys)
        {
            if (_keys == null)
                _keys = new List<string>();
            
            _keys.AddRange(keys.Select(x=>_serializer.Serialize(x)));
            _multikey = true;

            return this;
        }

        internal ViewQuery EncodedKeys(IEnumerable<string> keys)
        {
            if (_keys == null)
                _keys = new List<string>();

            _keys.AddRange(keys);
            _multikey = true;

            return this;
        }

        public ViewQuery EndKey(object key)
        {
            _endKey = _serializer.Serialize(key);
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

        public ViewQuery IncludeDocs()
        {
            _include_docs = true;
            return this;
        }

        public ViewQuery OrderDescending(bool desc)
        {
            _descending = desc;
            return this;
        }
        public ViewQuery OrderDescending()
        {
            return OrderDescending(true);
        }
        public ViewQuery GroupLevel(int level)
        { 
            _groupLevel = level.ToString();
            return this;
        }
        public ViewQuery Group(bool group)
        { 
            _group = group;
            return this;
        }
        public ViewQuery Group()
        { 
            return this.Group(true);
        }

        public ViewQuery Reduce(bool reduce)
        {
            _reduce = reduce;
            return this;
        }
        public ViewQuery Reduce()
        {
            return this.Reduce(true);
        }


        public string Method
        {
            get;
            private set;
        }

        public object RequestData
        {
            get;
            private set;
        }
        
        public ViewQuery SetUrlPattern(string pattern)
        {
            _urlpattern = pattern;
            return this;
        }
        public string GenerateQuery()
        {
            string q = "";

                q = string.Format(_urlpattern, _name, _view);

                Method = "GET";

            if (_keys != null)
            {
                if (!_multikey)
                {

                    RequestData = null;
                    if (string.IsNullOrEmpty(_endKey))
                    {
                        q = q.AddQueryStirng( "key", _keys.First());
                    }
                    else
                    {
                        if (_descending)
                        {
                            q = q.AddQueryStirng( "endkey", _keys.First());

                            q = q.AddQueryStirng( "startkey", _endKey);
                        }
                        else
                        {
                            q = q.AddQueryStirng( "startkey", _keys.First());

                            q = q.AddQueryStirng( "endkey", _endKey);
                        }
                    }
                }
                else
                {
                    RequestData = (new { keys = _keys });

                    Method = "POST";
                }
            }

            if (_take.HasValue)
                q = q.AddQueryStirng( "limit", _take.Value);

            if (_reduce.HasValue && !_reduce.Value)
                q = q.AddQueryStirng( "reduce", "false");

            if (!_reduce.HasValue || _reduce.Value)
            {
                if (!string.IsNullOrEmpty(_groupLevel))
                    q = q.AddQueryStirng("group_level", _groupLevel);

                if (_group.HasValue)
                {
                    if (_group.Value)
                        q = q.AddQueryStirng( "group", "true");
                    else
                        q = q.AddQueryStirng( "group", "false");
                }
            }
            
            if (_skip.HasValue)
                q = q.AddQueryStirng("skip", _skip.Value);
            if (_descending)
                q = q.AddQueryStirng( "descending", "true");
            if (_include_docs)
                q = q.AddQueryStirng( "include_docs", "true");
            return q;
        }
    }
}
