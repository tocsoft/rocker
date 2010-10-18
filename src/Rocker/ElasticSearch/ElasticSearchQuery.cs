using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.ElasticSearch
{
    public class ElasticSearchQuery
    {
        List<string> _fields = new List<string>();
        string _query = "";
        string _type;
        int _from = 0; //these are the defaults from elastic search
        int _size = 10;//these are the defaults from elastic search
        public ElasticSearchQuery()
        {
        }
        Dictionary<string, object> _highlightFields = new Dictionary<string, object>();
        public ElasticSearchQuery Field(string field)
        {
            _fields.Add(field);
            return this;
        }

        public ElasticSearchQuery Field(string field, float boost)
        {
            _fields.Add(field + "^" + boost);
            return this;
        }

        public ElasticSearchQuery Highlight(string field)
        {
            _highlightFields.Add(field, new { fragment_size = 100 });
            return this;
        }

        public ElasticSearchQuery Take(int take)
        {
            _size = take;
            return this;
        }

        public ElasticSearchQuery Skip(int skip)
        {
            _from = skip;
            return this;
        }


        public ElasticSearchQuery Query(string query)
        {
            _query = query;
            return this;
        }

        internal object GenerateQueryObject()
        {
            return new
            {
                from = _from,
                size = _size,
                highlight = new
                        {
                            fields = _highlightFields
                        },
                query = new
                {
                    query_string = new
                    {
                        fields = _fields,
                        query = _query,
                        dis_max = true,
                        fuzzy_prefix_length = 3
                    }
                }
            };
        }
    }
}
