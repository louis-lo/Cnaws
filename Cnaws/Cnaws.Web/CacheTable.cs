using Cnaws.Web.Caching;
using System;
using System.Collections;

namespace Cnaws.Web
{
    internal sealed class CacheTable<T>
    {
        private string _name;

        public CacheTable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _name = name;
        }

        public T this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                Hashtable table = AppCache.Instance.Get(_name) as Hashtable;
                if (table != null)
                {
                    object value = table[key];
                    if (value != null)
                        return (T)value;
                }
                return default(T);
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                Hashtable table = AppCache.Instance.Get(_name) as Hashtable;
                if (table == null)
                {
                    table = new Hashtable();
                    AppCache.Instance.Set(_name, table);
                }
                if (value == null)
                    table.Remove(key);
                else
                    table[key] = value;
            }
        }

        public static void Set(string name, object key, T value)
        {
            CacheTable<T> ct = new CacheTable<T>(name);
            ct[key] = value;
        }
        public static T Get(string name, object key)
        {
            CacheTable<T> ct = new CacheTable<T>(name);
            return ct[key];
        }
    }
}
