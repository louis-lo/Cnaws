using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;

namespace Cnaws.FriendLink.Modules
{
    [Serializable]
    public sealed class FriendLink : IdentityModule
    {
        [DataColumn(32)]
        public string Name = null;
        [DataColumn(256)]
        public string Url = null;
        [DataColumn(256)]
        public string Image = null;
        public int SortNum = 0;
        public bool Approved = false;

        private static string[] GetCacheName()
        {
            return new string[] { "FriendLink", "Module" };
        }
        private static void RemoveCache()
        {
            CacheProvider.Current.Set(GetCacheName(), null);
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Approved");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Approved", "Approved");
        }
        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Url))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Url))
                return DataStatus.Failed;
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
        
        public static FriendLink GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<FriendLink>(ds, P("Id", id));
        }
        public static SplitPageData<FriendLink> GetPage(DataSource ds, bool approved, int index, int size, int show = 8)
        {
            int count;
            IList<FriendLink> list = ExecuteReader<FriendLink>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("Approved", approved));
            return new SplitPageData<FriendLink>(index, size, list, count, show);
        }
        public static IList<FriendLink> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<FriendLink> result = CacheProvider.Current.Get<IList<FriendLink>>(key);
            if (result == null)
            {
                result = ExecuteReader<FriendLink>(ds, Os(Oa("SortNum"), Oa("Id")), P("Approved", true));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
    }
}
