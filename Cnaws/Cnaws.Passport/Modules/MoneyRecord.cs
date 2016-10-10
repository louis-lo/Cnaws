using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Passport.Modules
{
    [Serializable]
    public class MoneyRecord : LongIdentityModule
    {
        public long MemberId = 0L;
        [DataColumn(128)]
        public string Title = null;
        public int Type = 0;
        [DataColumn(64)]
        public string TargetId = null;
        public Money Value = 0;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "MemberId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "MemberId", "MemberId");
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static SplitPageData<MoneyRecord> GetPageByMember(DataSource ds, long memberId, long index, int size, int show = 8)
        {
            long count;
            IList<MoneyRecord> list = ExecuteReader<MoneyRecord>(ds, Os(Od("CreationDate"), Od("Id")), index, size, out count, P("MemberId", memberId));
            return new SplitPageData<MoneyRecord>(index, size, list, count, show);
        }
    }
}
