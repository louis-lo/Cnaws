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
    public class ProductSalesArea : NoIdentityModule
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        [DataColumn(true)]
        public long ProductId = 0;
        /// <summary>
        /// 省Id
        /// </summary>
        [DataColumn(true)]
        public int Province = 0;
        /// <summary>
        /// 市Id
        /// </summary>
        [DataColumn(true)]
        public int City = 0;
        /// <summary>
        /// 区/县Id
        /// </summary>
        [DataColumn(true)]
        public int County = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Area");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Area", "Province", "City", "County", "ProductId");

        }

        public static IList<ProductSalesArea> GetById(DataSource ds,long productId)
        {
            return Db<ProductSalesArea>.Query(ds).Select().Where(W("ProductId", productId)).ToList<ProductSalesArea>();
        }
        public static DataStatus DelById(DataSource ds, long productId)
        {
            if (Db<ProductSalesArea>.Query(ds).Delete().Where(W("ProductId", productId)).Execute() > 0)
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

    }
}