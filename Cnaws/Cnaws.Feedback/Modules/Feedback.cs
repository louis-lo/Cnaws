using Cnaws.Web;
using Cnaws.Data;
using System;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Feedback.Modules
{
    [Serializable]
    public sealed class Feedback : IdentityModule
    {
        public int CategoryId = 0;
        public int UserId = 0;
        [DataColumn(32)]
        public string UserName = null;
        [DataColumn(128)]
        public string Email = null;
        [DataColumn(16)]
        public string Phone = null;
        [DataColumn(16)]
        public string QQ = null;
        [DataColumn(2000)]
        public string Content = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "CategoryId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "CategoryId", "CategoryId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(UserName))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Email))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        public static DataStatus Insert(DataSource ds, int categoryId, int userid, string username, string email, string phone, string qq, string content)
        {
            return (new Feedback() { CategoryId = categoryId, UserId = userid, UserName = username, Email = email, Phone = phone, QQ = qq, Content = content, CreationDate = DateTime.Now }).Insert(ds);
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static DataStatus Delete(DataSource ds, int id)
        {
            return (new Feedback() { Id = id }).Delete(ds);
        }

        public static Feedback GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<Feedback>(ds, P("Id", id));
        }
        public static int GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return (int)ExecuteCount<Feedback>(ds, P("CategoryId", categoryId));
        }
        public static SplitPageData<Feedback> GetPage(DataSource ds, int categoryId, int index, int size, int show = 8)
        {
            int count;
            IList<Feedback> list;
            if (categoryId == 0)
                list = ExecuteReader<Feedback>(ds, Os(Od("CreationDate"), Od("Id")), index, size, out count);
            else
                list = ExecuteReader<Feedback>(ds, Os(Od("Top"), Od("CreationDate"), Od("Id")), index, size, out count, P("CategoryId", categoryId));
            return new SplitPageData<Feedback>(index, size, list, count, show);
        }
    }
}
