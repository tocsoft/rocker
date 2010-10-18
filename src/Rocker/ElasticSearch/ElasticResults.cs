using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.ElasticSearch
{
    public class ElasticResults<T>
    {
        public ElasticHitList<T> hits { get; set; }
    }

    public class ElasticHitList<T>
    {
        public int total { get; set; }
        public float max_score { get; set; }
        public List<ElasticHit<T>> hits { get; set; }

    }
    public class ElasticHit<T>
    {
        public string _index {get;set;}
        public string _type {get;set;}
        public string _id {get;set;}
        public float _score {get;set;}
        public T _source { get; set; }
        public Dictionary<string, List<string>> highlight { get; set; }
        
    }
}
