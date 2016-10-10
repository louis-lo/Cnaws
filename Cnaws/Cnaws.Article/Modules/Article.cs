using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;

namespace Cnaws.Article.Modules
{
    [Serializable]
    public sealed class Article : IdentityModule
    {
        [DataColumn(128)]
        public string Title = null;
        [DataColumn(256)]
        public string Image = null;
        public string Content = null;
        public int CategoryId = 0;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        [DataColumn(2000)]
        public string Keywords = null;
        [DataColumn(2000)]
        public string Description = null;
        public bool Visibility = true;
        public bool Top = false;
        public int Style = 0;
        [DataColumn(16)]
        public string Color = null;
        [DataColumn(32)]
        public string Author = null;
        [DataColumn(256)]
        public string Referer = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "CategoryId");
            DropIndex(ds, "CategoryIdVisibility");
            DropIndex(ds, "Referer");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "CategoryId", "CategoryId");
            CreateIndex(ds, "CategoryIdVisibility", "CategoryId", "Visibility");
            CreateIndex(ds, "Referer", "Referer");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Title))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            if ("#000".Equals(Color))
                Color = null;
            if (ArticleCategory.GetCountByParent(ds, CategoryId) > 0)
                return DataStatus.Exist;
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, string title, string image, string content, int categoryId, DateTime creationDate, string keywords, string description, bool visibility, bool top, int style, string color, string author, string referer)
        {
            return (new Article() { Title = title, Image = image, Content = content, CategoryId = categoryId, CreationDate = creationDate, Keywords = keywords, Description = description, Visibility = visibility, Top = top, Style = style, Color = color, Author = author, Referer = referer }).Insert(ds);
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "CreationDate");
            if (string.IsNullOrEmpty(Title))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        public static DataStatus Update(DataSource ds, int id, string title, string image, string content, int categoryId, DateTime creationDate, string keywords, string description, bool visibility, bool top, int style, string color, string author, string referer)
        {
            return (new Article() { Id = id, Title = title, Image = image, Content = content, CategoryId = categoryId, CreationDate = creationDate, Keywords = keywords, Description = description, Visibility = visibility, Top = top, Style = style, Color = color, Author = author, Referer = referer }).Update(ds);
        }
        public static DataStatus Update(DataSource ds, int id, string title, bool visibility, bool top)
        {
            return (new Article() { Id = id, Title = title, Visibility = visibility, Top = top }).Update(ds, ColumnMode.Include, "Title", "Visibility", "Top");
        }

        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new Article() { Id = id }).Delete(ds);
        }

        public static Article GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<Article>(ds, P("Id", id));
        }
        public static int GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return (int)ExecuteCount<Article>(ds, P("CategoryId", categoryId));
        }
        public static SplitPageData<Article> GetPage(DataSource ds, int categoryId, int index, int size, int show = 8)
        {
            int count;
            IList<Article> list;
            if (categoryId == 0)
                list = ExecuteReader<Article>(ds, Os(Od("CreationDate"), Od("Id")), index, size, out count);
            else
                list = ExecuteReader<Article>(ds, Os(Od("Top"), Od("CreationDate"), Od("Id")), index, size, out count, P("CategoryId", categoryId));
            return new SplitPageData<Article>(index, size, list, count, show);
        }
        public static IList<Article> GetAllTop(DataSource ds, int categoryId)
        {
            return ExecuteReader<Article>(ds, Os(Od("CreationDate"), Od("Id")), P("Top", true) & P("CategoryId", categoryId));
        }
        public static IList<Article> GetTop(DataSource ds, int categoryId, int count)
        {
            return Db<Article>.Query(ds)
                .Select()
                .Where(W("Visibility", true) & W("CategoryId", categoryId))
                .OrderBy(D("Id"),D("CreationDate"))
                .ToList<Article>(count);
        }
        public static SplitPageData<Article> GetPageByRoot(DataSource ds, int categoryId, long index, int size, int show = 8)
        {
            long count;
            IList<Article> list = Db<Article>.Query(ds)
                .Select()
                .Where(W("CategoryId").InSelect<ArticleCategory>("Id").Where(W("ParentId", categoryId)).Result())
                .OrderBy(D("CreationDate"), D("Id"))
                .ToList<Article>(size, index, out count);
            return new SplitPageData<Article>(index, size, list, count, show);
        }
    }
}
