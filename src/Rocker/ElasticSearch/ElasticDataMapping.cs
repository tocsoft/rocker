using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Rocker.ElasticSearch
{
    public class ElasticDataClassMapping<T>
    {
        string _type;

        Dictionary<string, object> _properties = new Dictionary<string, object>();
        public ElasticDataClassMapping(string type)
        {
            _type = type;
        }

        public ElasticDataClassMapping<T> Map(
            Expression<Func<T, object>> property,
            Func<ElasticDataPropertyMapping,
                ElasticDataPropertyMapping> map)
        {
            MemberExpression memberExpression = null;

            if (property.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = property.Body as MemberExpression;
            }


            var prop = map.Invoke(new ElasticDataPropertyMapping(memberExpression.Member.Name, memberExpression.Type));
            _properties.Add(memberExpression.Member.Name, prop.GetMappingObject());

            return this;
        }
       
        internal object GetMappingObject()
        {
            var obj =  new Dictionary<string, object>();

            obj.Add(_type, new
            { 
                properties = _properties
            });
            return obj;
        }
    }

    public class ElasticDataPropertyMapping
    {
        private string _propertyName;
        private Type _propertyType;
        private bool _store;
        private float _boost = 1;
        private TermVectors _term_vector;
        private string _analyzer;
        
        public ElasticDataPropertyMapping(string propertyName, Type type)
        {
            _propertyName = propertyName;
            _propertyType = type;
        }
        public ElasticDataPropertyMapping Store()
        {
            return Store(true);
        }
        public ElasticDataPropertyMapping Store(bool store)
        {
            _store = store;
            return this;
        }
        public ElasticDataPropertyMapping Boost(float boost)
        {
            _boost = boost;
            return this;
        }
        public ElasticDataPropertyMapping Analyzer(string analyzer)
        {
            _analyzer = analyzer;
            return this;
        }
        public enum TermVectors
        {
            no, yes, with_offsets, with_positions, with_positions_offsets
        }
        public ElasticDataPropertyMapping TermVector(TermVectors vector)
        {
            _term_vector = vector;
            return this;
        }

        public ElasticDataPropertyMapping EnableHighlighting()
        {            
            return TermVector(TermVectors.with_positions_offsets).Store(true);
        } 

        private string GetElasticType()
        {
            switch (_propertyType.Name)
            {
                case "String":
                    return "string";
                case "DateTime":
                    return "date";
                default:
                    return "object";
            }
        }
        internal object GetMappingObject()
        {
            var obj = new Dictionary<string, object>();
            obj.Add("type", GetElasticType());
            obj.Add("store", _store ? "yes" : "no");
            obj.Add("boost", _boost);
            obj.Add("term_vector", _term_vector);
            if(!string.IsNullOrEmpty(_analyzer))
                obj.Add("analyzer", _analyzer);
            return obj;
            
        }
    }
}
