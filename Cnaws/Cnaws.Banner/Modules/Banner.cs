using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;

namespace Cnaws.Banner.Modules
{
    [Serializable]
    public sealed class Banner : IdentityModule
    {
        [DataColumn(128)]
        public string Title = null;
        [DataColumn(256)]
        public string Image = null;
        [DataColumn(256)]
        public string Url = null;
        public int SortNum = 0;
        public bool Visibility = true;

        private static string[] GetCacheName()
        {
            return new string[] { "Banner", "Module" };
        }
        private static void RemoveCache()
        {
            CacheProvider.Current.Set(GetCacheName(), null);
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Visibility");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Visibility", "Visibility");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Image))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        public static SplitPageData<Banner> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<Banner> list = ExecuteReader<Banner>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count);
            return new SplitPageData<Banner>(index, size, list, count, show);
        }
        public static IList<Banner> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<Banner> result = CacheProvider.Current.Get<IList<Banner>>(key);
            if (result == null)
            {
                result = ExecuteReader<Banner>(ds, Os(Oa("SortNum"), Oa("Id")), P("Visibility", true));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
    }
}
