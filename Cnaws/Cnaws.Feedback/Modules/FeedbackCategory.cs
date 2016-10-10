using Cnaws.Data;
using Cnaws.Web;
using System;
using System.Collections.Generic;

namespace Cnaws.Feedback.Modules
{
    [Serializable]
    public sealed class FeedbackCategory : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        public bool IsSys = false;
        public int SortNum = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "IsSys");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "IsSys", "IsSys");
            (new FeedbackCategory()
            {
                Name = "在线留言",
                IsSys = true,
                SortNum = 0
            }).Insert(ds);
        }

        private static string[] GetCacheName()
        {
            return new string[] { "FeedbackCategory", "Module" };
        }
        private static void RemoveCache()
        {
            CacheProvider.Current.Set(GetCacheName(), null);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, string name, int sortNum)
        {
            return (new FeedbackCategory() { Name = name, IsSys = false, SortNum = sortNum }).Insert(ds);
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        public static DataStatus Update(DataSource ds, int id, string name, int sortNum)
        {
            return (new FeedbackCategory() { Id = id, Name = name, SortNum = sortNum }).Update(ds, ColumnMode.Include, "Name", "SortNum");
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (ExecuteScalar<FeedbackCategory, bool>(ds, "IsSys", P("Id", Id)))
                return DataStatus.Failed;
            if (Feedback.GetCountByCategoryId(ds, Id) > 0)
                return DataStatus.Exist;
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new FeedbackCategory() { Id = id }).Delete(ds);
        }

        public static FeedbackCategory GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<FeedbackCategory>(ds, P("Id", id));
        }
        public static SplitPageData<FeedbackCategory> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<FeedbackCategory> list = ExecuteReader<FeedbackCategory>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count);
            return new SplitPageData<FeedbackCategory>(index, size, list, count, show);
        }
        public static IList<FeedbackCategory> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<FeedbackCategory> result = CacheProvider.Current.Get<IList<FeedbackCategory>>(key);
            if (result == null)
            {
                result = ExecuteReader<FeedbackCategory>(ds);
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
    }
}
