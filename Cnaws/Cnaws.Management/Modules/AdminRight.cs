using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Management.Modules
{
    [Serializable]
    public sealed class AdminRight : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        public string Right = null;

        private static string[] GetCacheName()
        {
            return new string[] { "AdminRight", "Module" };
        }

        private void Insert(DataSource ds, string name, string right)
        {
            (new AdminRight()
            {
                Name = name,
                Right = right
            }).Insert(ds);
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            foreach (ManagementRight r in ManagementRights.GetAll(HttpContext.Current))
                Insert(ds, r.Name, r.Right);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Right))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(), null);
            return DataStatus.Success;
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Right))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(), null);
            return DataStatus.Success;
        }

        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(), null);
            return DataStatus.Success;
        }

        public static IList<AdminRight> GetAll(DataSource ds)
        {
            return ExecuteReader<AdminRight>(ds, Cs("Id", "Name"));
        }
        private static Dictionary<string, int> GetArray(DataSource ds)
        {
            string[] key = GetCacheName();
            Dictionary<string, int> dict = CacheProvider.Current.Get<Dictionary<string, int>>(key);
            if (dict == null)
            {
                dict = new Dictionary<string, int>();
                IList<AdminRight> list = ExecuteReader<AdminRight>(ds, Cs("Id", "Right"));
                foreach (AdminRight right in list)
                    dict.Add(right.Right, right.Id);
                CacheProvider.Current.Set(key, dict);
            }
            return dict;
        }
        public static int GetIdByRight(DataSource ds, string right)
        {
            int value;
            Dictionary<string, int> dict = GetArray(ds);
            if (dict.TryGetValue(right, out value))
                return value;
            return 0;
        }
    }
}
