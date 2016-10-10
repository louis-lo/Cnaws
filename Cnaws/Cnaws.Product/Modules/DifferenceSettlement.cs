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
    public class DifferenceSettlement : NoIdentityModule
    {
        public enum EProductType
        {
            /// <summary>
            /// 常规
            /// </summary>
            Routine = 0,
            /// <summary>
            /// 团购
            /// </summary>
            GroupBuy,
            /// <summary>
            /// 抢购
            /// </summary>
            PanicBuying,
            /// <summary>
            /// 批发
            /// </summary>
            Wholesale,
            /// <summary>
            /// 进货
            /// </summary>
            Purchase

        }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataColumn(true, 36)]
        public string OrderId = null;
        /// <summary>
        /// 产品编号
        /// </summary>
        [DataColumn(true)]
        public long ProductId = 0L;
        /// <summary>
        /// 产品所属类型
        /// </summary>
        public EProductType ProductType = EProductType.Routine;
        /// <summary>
        /// 当类型为固定价格时的结算价格
        /// </summary>
        public Money CostPrice = 0;
        /// <summary>
        /// 省级加盟商Id
        /// </summary>
        public long CountyId = 0;
        /// <summary>
        /// 县级加盟商应结算的钱
        /// </summary>
        public Money CountyPrice = 0;
        /// <summary>
        /// 上级介绍人Id
        /// </summary>
        public long ParentId = 0;
        /// <summary>
        /// 上级介绍人应结算的钱
        /// </summary>
        public Money ParentPrice = 0;
        /// <summary>
        /// 销售人的Id
        /// </summary>
        public long DotId = 0;
        /// <summary>
        /// 销售人应结算的钱
        /// </summary>
        public Money DotPrice = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "OrderId");
            DropIndex(ds, "ProductId");
            DropIndex(ds, "OrderIdAndProductId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "OrderId", "OrderId");
            CreateIndex(ds, "ProductId", "ProductId");
            CreateIndex(ds, "OrderIdAndProductId", "OrderId", "ProductId");
        }

        public static IList<DifferenceSettlement> GetListbyOrderId(DataSource ds, string orderid)
        {
            return Db<DifferenceSettlement>.Query(ds).Select().Where(W("OrderId", orderid)).ToList<DifferenceSettlement>();
        }

        public static DifferenceSettlement GetbyId(DataSource ds, string orderid, long productid)
        {
            return Db<DifferenceSettlement>.Query(ds).Select().Where(W("OrderId", orderid) & W("ProductId", productid)).First<DifferenceSettlement>();
        }
    }
}
