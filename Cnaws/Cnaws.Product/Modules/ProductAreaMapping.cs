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
    public class ProductAreaMapping : NoIdentityModule
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
        /// <summary>
        /// 成本价
        /// </summary>
        public Money CostPrice = 0;
        /// <summary>
        /// 县级成本价
        /// </summary>
        public Money CountyPrice = 0;
        /// <summary>
        /// 网点成本价//(进货宝)网点进货价
        /// </summary>
        public Money DotPrice = 0;
        /// <summary>
        /// 零售价
        /// </summary>
        public Money Price = 0;
        /// <summary>
        /// 是否销售中
        /// </summary>
        public bool Saled = false;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Area");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Area", "Province", "City", "County", "ProductId");
        }

        public static ProductAreaMapping GetById(DataSource ds,long id,int province,int city,int county)
        {
            return Db<ProductAreaMapping>.Query(ds).Select().Where(W("ProductId", id) & W("Province", province) & W("City", city) & W("County", county)).First<ProductAreaMapping>();
        }

        public static DataStatus ModPrice(DataSource ds,long id, int province, int city, int county,string ModField,Money price)
        {
            if (Db<ProductAreaMapping>.Query(ds).Update().Set(ModField, price).Where(W("ProductId", id) & W("Province", province) & W("City", city) & W("County", county)).Execute() > 0)
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

        public static DataStatus ModSaled(DataSource ds, long id, int province, int city, int county, bool saled)
        {
            if (Db<ProductAreaMapping>.Query(ds).Update().Set("Saled", saled).Where(W("ProductId", id) & W("Province", province) & W("City", city) & W("County", county)).Execute() > 0)
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

    }
}