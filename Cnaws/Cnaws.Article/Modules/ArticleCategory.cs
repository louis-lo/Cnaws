using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;

namespace Cnaws.Article.Modules
{
    [Serializable]
    public sealed class ArticleCategory : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        public int ParentId = 0;
        public bool IsSys = false;
        public int SortNum = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ParentId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ParentId", "ParentId");
            (new ArticleCategory()
            {
                Name = "系统公告",
                ParentId = 0,
                IsSys = true,
                SortNum = 0
            }).Insert(ds);
        }

        private static string[] GetCacheName(int id)
        {
            return new string[] { "ArticleCategory", "Module", id.ToString() };
        }
        private static void RemoveCache(int id, int parentId)
        {
            CacheProvider.Current.Set(GetCacheName(-1), null);
            if (parentId > -1)
                CacheProvider.Current.Set(GetCacheName(parentId), null);
            CacheProvider.Current.Set(GetCacheName(id), null);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache(Id, ParentId);
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, string name, int parentId, int sortNum)
        {
            return (new ArticleCategory() { Name = name, ParentId = parentId, IsSys = false, SortNum = sortNum }).Insert(ds);
        }

        private void CheckParentId(DataSource ds)
        {
            if (ParentId == 0)
                ParentId = ExecuteScalar<ArticleCategory, int>(ds, "ParentId", P("Id", Id));
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            CheckParentId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache(Id, ParentId);
            return DataStatus.Success;
        }
        public static DataStatus Update(DataSource ds, int id, string name, int sortNum)
        {
            return (new ArticleCategory() { Id = id, Name = name, SortNum = sortNum }).Update(ds, ColumnMode.Include, "Name", "SortNum");
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (ExecuteScalar<ArticleCategory, bool>(ds, "IsSys", P("Id", Id)))
                return DataStatus.Failed;
            if (ExecuteCount<ArticleCategory>(ds, P("ParentId", Id)) > 0)
                return DataStatus.Exist;
            if (Article.GetCountByCategoryId(ds, Id) > 0)
                return DataStatus.Exist;
            CheckParentId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache(Id, ParentId);
            return DataStatus.Success;
        }
        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new ArticleCategory() { Id = id }).Delete(ds);
        }

        public static ArticleCategory GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<ArticleCategory>(ds, P("Id", id));
        }
        public static SplitPageData<ArticleCategory> GetPage(DataSource ds, int parentId, int index, int size, int show = 8)
        {
            int count;
            IList<ArticleCategory> list = ExecuteReader<ArticleCategory>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("ParentId", parentId));
            return new SplitPageData<ArticleCategory>(index, size, list, count, show);
        }
        public static IList<ArticleCategory> GetAll(DataSource ds, int parentId)
        {
            string[] key = GetCacheName(parentId);
            IList<ArticleCategory> result = CacheProvider.Current.Get<IList<ArticleCategory>>(key);
            if (result == null)
            {
                if (parentId == -1)
                    result = ExecuteReader<ArticleCategory>(ds);
                else
                    result = ExecuteReader<ArticleCategory>(ds, Os(Oa("SortNum"), Oa("Id")), P("ParentId", parentId));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static IList<ArticleCategory> GetTop(DataSource ds, int parentId, int count)
        {
            return Db<ArticleCategory>.Query(ds)
                .Select()
                .Where(W("IsSys", false) & W("ParentId", parentId))
                .OrderBy(A("SortNum"), A("Id"))
                .ToList<ArticleCategory>(count);
        }
        public static long GetCountByParent(DataSource ds, int parentId)
        {
            return ExecuteCount<ArticleCategory>(ds, P("ParentId", parentId));
        }
    }
}
