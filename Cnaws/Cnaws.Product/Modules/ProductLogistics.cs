using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;
using System.Collections.Generic;
namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class ProductLogistics : NoIdentityModule
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataColumn(true, 36)]
        public string OrderId = null;
        /// <summary>
        /// 快递名称
        /// </summary>
        public string ProviderKey = null;
        /// <summary>
        /// 快递名称
        /// </summary>
        public string ProviderName = null;
        /// <summary>
        /// 快递单号
        /// </summary>
        public string BillNo = null;

        /// <summary>
        /// 快递明细
        /// </summary>
        public string ProviderDetailed = null;

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(OrderId))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(ProviderName))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(BillNo))
                return DataStatus.Failed;
            ds.Begin();
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            ds.Commit();
            return DataStatus.Success;
        }
        protected override void OnInsertFailed(DataSource ds)
        {
            ds.Rollback();
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static ProductLogistics GetByOrder(DataSource ds,string orderId)
        {
            return Db<ProductLogistics>.Query(ds)
                .Select()
                .Where(W("OrderId", orderId))
                .First<ProductLogistics>();
        }

        public static int UpdateProviderDetailed(DataSource ds, ProductLogistics model)
        {
            return Db<ProductLogistics>.Query(ds)
                .Update(model, O<ProductLogistics>("ProviderDetailed"));
        }
    }
}
