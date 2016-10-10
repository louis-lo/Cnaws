using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Pay.Modules
{
    [Serializable]
    public sealed class PayLog : LongIdentityModule
    {
        [DataColumn(32)]
        public string Provider = null;
        [DataColumn(32)]
        public string TradeNo = null;
        public string Message = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Provider");
            DropIndex(ds, "TradeNo");
            DropIndex(ds, "ProviderTradeNo");
            DropIndex(ds, "CreationDate");
            DropIndex(ds, "ProviderCreationDate");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Provider", "Provider");
            CreateIndex(ds, "TradeNo", "TradeNo");
            CreateIndex(ds, "ProviderTradeNo", "Provider", "TradeNo");
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

        public static SplitPageData<PayLog> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<PayLog> list = ExecuteReader<PayLog>(ds, Os(Od("CreationDate")), index, size, out count);
            return new SplitPageData<PayLog>(index, size, list, count, show);
        }
    }
}
