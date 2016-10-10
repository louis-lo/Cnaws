using Cnaws.Web.Caching;
using System;
using System.Web;

namespace Cnaws.Web
{
    public abstract class CacheProvider : IDisposable
    {
        private bool disposed;

        protected CacheProvider()
        {
            disposed = false;
        }

        public object this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public static CacheProvider Current
        {
            get
            {
                switch (Settings.Instance.CacheMode)
                {
                    case Configuration.CacheMode.Application: return AppCache.Instance;
                    case Configuration.CacheMode.File: return FileCache.Instance;
                    case Configuration.CacheMode.MMFile: return MMFileCache.Instance;
                    case Configuration.CacheMode.Sql: return SqlCache.Instance;
                }
                return null;
            }
        }

        protected abstract string FormatKey(string key);
        protected abstract string FormatKeys(params string[] keys);
        protected abstract object GetImpl(string key);
        protected abstract void SetImpl(string key, object value);
        protected abstract void DeleteImpl(string key);
        public object Get(string key)
        {
            return GetImpl(FormatKey(key));
        }
        public object Get(string[] keys)
        {
            return GetImpl(FormatKeys(keys));
        }
        public T Get<T>(string key)
        {
            object value = GetImpl(FormatKey(key));
            if (value != null)
                return (T)value;
            return default(T);
        }
        public T Get<T>(string[] keys)
        {
            object value = GetImpl(FormatKeys(keys));
            if (value != null)
                return (T)value;
            return default(T);
        }
        public void Set(string key, object value = null)
        {
            if (value != null)
                SetImpl(FormatKey(key), value);
            else
                DeleteImpl(FormatKey(key));
        }
        public void Set(string[] keys, object value = null)
        {
            if (value != null)
                SetImpl(FormatKeys(keys), value);
            else
                DeleteImpl(FormatKeys(keys));
        }
        public abstract void Clear();

        public void Dispose()
        {
            DisposeImpl(true);
            GC.SuppressFinalize(this);
        }
        private void DisposeImpl(bool disposing)
        {
            if (!disposed)
            {
                Dispose(disposing);
                disposed = true;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
        }
        ~CacheProvider()
        {
            DisposeImpl(false);
        }
    }
}
