using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Management.Modules
{
    [Serializable]
    public sealed class AdminRole : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        public string Rights = null;

        protected override void OnInstallAfter(DataSource ds)
        {
            (new AdminRole()
            {
                Name = "超级管理员",
                Rights = "-1"
            }).Insert(ds);
        }

        private static string[] GetCacheName(int id)
        {
            return new string[] { "AdminRole", "Module", id.ToString() };
        }
        private static void RemoveCache(int id)
        {
            CacheProvider.Current.Set(GetCacheName(0), null);
            CacheProvider.Current.Set(GetCacheName(id), null);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Rights))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache(Id);
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, string name, string rights)
        {
            return (new AdminRole() { Name = name, Rights = rights }).Insert(ds);
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Rights))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache(Id);
            return DataStatus.Success;
        }
        public static DataStatus Update(DataSource ds, int id, string name, string rights)
        {
            return (new AdminRole() { Id = id, Name = name, Rights = rights }).Update(ds);
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (Admin.GetCountByRoleId(ds, Id) > 0)
                return DataStatus.Exist;
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache(Id);
            return DataStatus.Success;
        }
        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new AdminRole() { Id = id }).Delete(ds);
        }

        public static AdminRole GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<AdminRole>(ds, Cs("Id", "Name", "Rights"), P("Id", id));
        }
        public static IList<AdminRole> GetAll(DataSource ds)
        {
            return ExecuteReader<AdminRole>(ds, Cs("Id", "Name", "Rights"));
        }
        public static Dictionary<string, string> GetAllNames(DataSource ds)
        {
            string[] name = GetCacheName(0);
            Dictionary<string, string> dict = CacheProvider.Current.Get<Dictionary<string, string>>(name);
            if (dict == null)
            {
                dict = new Dictionary<string, string>();
                IList<AdminRole> list = ExecuteReader<AdminRole>(ds, Cs("Id", "Name"));
                foreach (AdminRole role in list)
                    dict.Add(role.Id.ToString(), role.Name);
                CacheProvider.Current.Set(name, dict);
            }
            return dict;
        }
        public static SplitPageData<AdminRole> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<AdminRole> list = ExecuteReader<AdminRole>(ds, Cs("Id", "Name", "Rights"), Os(Oa("Id")), index, size, out count);
            return new SplitPageData<AdminRole>(index, size, list, count, show);
        }
        
        private static List<int> GetArray(DataSource ds, int id)
        {
            string[] name = GetCacheName(id);
            List<int> list = CacheProvider.Current.Get<List<int>>(name);
            if (list == null)
            {
                list = new List<int>();
                string rights = ExecuteScalar<AdminRole, string>(ds, "Rights", P("Id", id));
                foreach (string right in rights.Split(','))
                    list.Add(int.Parse(right));
                CacheProvider.Current.Set(name, list);
            }
            return list;
        }
        public static bool HasRight(DataSource ds, int id, string right)
        {
            List<int> arr = GetArray(ds, id);
            if (arr.Count == 1 && arr[0] == -1)
                return true;
            int rid = AdminRight.GetIdByRight(ds, right);
            if (rid > 0)
            {
                foreach (int a in arr)
                {
                    if (a == rid)
                        return true;
                }
            }
            return false;
        }
    }
}
