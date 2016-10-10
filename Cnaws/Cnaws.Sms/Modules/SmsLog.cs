using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Sms.Modules
{
    [Serializable]
    public sealed class SmsLog : NoIdentityModule
    {
        [DataColumn(true)]
        public Guid Id = Guid.Empty;
        [DataColumn(32)]
        public string Provider = null;
        public string Message = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Provider");
            DropIndex(ds, "CreationDate");
            DropIndex(ds, "ProviderCreationDate");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Provider", "Provider");
            CreateIndex(ds, "CreationDate", "CreationDate");
            CreateIndex(ds, "ProviderCreationDate", "Provider", "CreationDate");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Provider))
                return DataStatus.Failed;
            Provider = Provider.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static SplitPageData<SmsLog> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<SmsLog> list = ExecuteReader<SmsLog>(ds, Os(Od("CreationDate")), index, size, out count);
            return new SplitPageData<SmsLog>(index, size, list, count, show);
        }
    }
}
