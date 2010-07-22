using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{
    public class CouchRevisionStore
    {

        Dictionary<object, RevisionInfo> _info = new Dictionary<object, RevisionInfo>();
        internal Dictionary<object, RevisionInfo> store
        {
            get
            {
                if (_info != null)
                    return _info;
                if (System.Web.HttpContext.Current != null) 
                { 
                    return System.Web.HttpContext.Current.Items["CouchRevisionInfoStore"] as Dictionary<object, RevisionInfo>
                }

                _info = new Dictionary<object,RevisionInfo>();
                return _info;
            }
        }

        public RevisionInfo Lookup(object obj)
        {
            if (store.ContainsKey(store))
                return store[store];
            else
                return null;
        }

        public void Update(object item, RevisionInfo info)
        {
            if (store.ContainsKey(item))
                store[item] = info;
            else
                store.Add(item, info);
        }
    }
}
