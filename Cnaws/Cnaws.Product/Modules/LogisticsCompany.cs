using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cnaws.Data;
using Cnaws.Web;
using Cnaws.Data.Query;
namespace Cnaws.Product.Modules
{
    [Serializable]
    public class LogisticsCompany : IdentityModule
    {
        [DataColumn(64)]
        public string Name = null;
        [DataColumn(64)]
        public string NameCode = null;

        public bool IsEnable = true;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Name");
            DropIndex(ds, "NameCode");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Name", "Name");
            CreateIndex(ds, "NameCode", "NameCode");
        }

        public static IList<LogisticsCompany> GetAll(DataSource ds)
        {
            return Db<LogisticsCompany>.Query(ds).Select().Where(W("IsEnable", true)).ToList<LogisticsCompany>();
        }

        public static SplitPageData<LogisticsCompany> GetList(DataSource ds, string keyword, int index, int size, int show = 8)
        {
            long count;
            IList<LogisticsCompany> list;
            DbWhereQueue where = null;
            if (keyword != "_")
                where = W("Name", keyword, DbWhereType.Like);
            else
                where = W("Id", 0, DbWhereType.NotEqual);
            list = Db<LogisticsCompany>.Query(ds).Select().Where(where).OrderBy(D("Id")).ToList<LogisticsCompany>(size, index, out count);
            return new SplitPageData<LogisticsCompany>(index, size, list, count, show);
        }

    }
}
