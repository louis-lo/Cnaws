using System;
using System.Web;

namespace Cnaws.Web.Caching
{
    internal sealed class AppCache : CacheProvider
    {
        public static readonly AppCache Instance;

        static AppCache()
        {
            Instance = new AppCache();
        }
        private AppCache()
        {
        }

        protected override string FormatKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return key;
        }
        protected override string FormatKeys(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            return string.Join(".", keys);
        }
        protected override object GetImpl(string key)
        {
            return HttpContext.Current.Application[key];
        }
        protected override void SetImpl(string key, object value)
        {
            HttpContext.Current.Application[key] = value;
        }
        protected override void DeleteImpl(string key)
        {
            HttpContext.Current.Application.Remove(key);
        }
        public override void Clear()
        {
            HttpContext.Current.Application.Clear();
        }
    }
}
