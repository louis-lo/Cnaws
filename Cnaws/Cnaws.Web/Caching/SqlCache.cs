using Cnaws.Data;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using M = Cnaws.Web.Modules;

namespace Cnaws.Web.Caching
{
    internal sealed class SqlCache : CacheProvider
    {
        public static readonly SqlCache Instance;
        private string _provider;

        static SqlCache()
        {
            Instance = new SqlCache();
        }
        private SqlCache()
        {
            _provider = Settings.Instance.CacheProvider;
        }

        private DataSource GetDataSource()
        {
            IDataController ctl = Application.Current.Controller as IDataController;
            if (ctl != null && string.Equals(ctl.DataSource.Name, _provider))
                return new DataSource(ctl.DataSource);
            return new DataSource(_provider);
        }
        public override void Clear()
        {
            using (DataSource ds = GetDataSource())
                DbTable.TruncateTable<M.SqlCache>(ds);
        }

        protected override string FormatKey(string key)
        {
            return key;
        }

        protected override string FormatKeys(params string[] keys)
        {
            return string.Join(".", keys);
        }

        protected override object GetImpl(string key)
        {
            M.SqlCache cache;
            using (DataSource ds = GetDataSource())
                cache = M.SqlCache.Get(ds, key);
            if (cache != null)
            {
                using (MemoryStream ms = new MemoryStream(cache.Value))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(ms);
                }
            }
            return null;
        }

        protected override void SetImpl(string key, object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(ms, value);
                using (DataSource ds = GetDataSource())
                    M.SqlCache.Set(ds, key, ms.ToArray());
            }
        }

        protected override void DeleteImpl(string key)
        {
            using (DataSource ds = GetDataSource())
                M.SqlCache.Delete(ds, key);
        }
    }
}
