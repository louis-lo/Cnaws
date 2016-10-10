using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    public class ProductBrandMapping : NoIdentityModule
    {
        [DataColumn(true)]
        public int BrandId = 0;
        [DataColumn(true)]
        public int CategoryId = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "BrandId");
            DropIndex(ds, "CategoryId");
            DropIndex(ds, "BrandIdAndCategoryId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "BrandId", "BrandId");
            CreateIndex(ds, "CategoryId", "CategoryId");
            CreateIndex(ds, "BrandIdAndCategoryId", "BrandId", "CategoryId");
        }

        public static IList<ProductCategory> GetCategoryListByBrandId(DataSource ds, int brandid)
        {
            return Db<ProductBrandMapping>.Query(ds)
                .Select(S<ProductCategory>())
                .InnerJoin(O<ProductBrandMapping>("CategoryId"), O<ProductCategory>("Id"))
                .Where(W<ProductBrandMapping>("BrandId", brandid))
                .OrderBy(D<ProductCategory>("SortNum"))
                .ToList<ProductCategory>();
        }

        public static IList<ProductBrand> GetBrandListByCategoryId(DataSource ds, int categoryid)
        {
            return Db<ProductBrandMapping>.Query(ds)
                .Select(S<ProductBrand>())
                .InnerJoin(O<ProductBrandMapping>("BrandId"), O<ProductBrand>("Id"))
                .Where(W<ProductBrandMapping>("CategoryId", categoryid))
                .OrderBy(D<ProductBrand>("SortNum"))
                .ToList<ProductBrand>();
        }
        public static IList<ProductBrandMapping> GetByBrandId(DataSource ds, int brandId)
        {
            return Db<ProductBrandMapping>.Query(ds)
                .Select()
                .Where(W("BrandId", brandId))
                .ToList<ProductBrandMapping>();
        }
        public static IList<ProductBrand> GetBrandListByCategoryIdAndScreen(DataSource ds, int categoryid)
        {
            return Db<ProductBrandMapping>.Query(ds)
                .Select(S<ProductBrand>())
                .InnerJoin(O<ProductBrandMapping>("BrandId"), O<ProductBrand>("Id"))
                .Where(W<ProductBrandMapping>("CategoryId", categoryid) & W<ProductBrand>("Screen", true))
                .OrderBy(D<ProductBrand>("SortNum"))
                .ToList<ProductBrand>();
        }

        public static DataStatus Add(DataSource ds, ProductBrandMapping brandmapping)
        {
            if (Db<ProductBrandMapping>.Query(ds)
                .Select().Where(W("BrandId", brandmapping.BrandId) & W("CategoryId", brandmapping.CategoryId))
                .Count() > 0)
                return DataStatus.Success;
            else
            {
                return brandmapping.Insert(ds);
            }
        }
        public static DataStatus Del(DataSource ds, List<int> ids, int brandid)
        {
            if (ids.Count > 0)
                Db<ProductBrandMapping>.Query(ds).Delete().Where(W("BrandId", brandid) & W("CategoryId", ids.ToArray(), DbWhereType.In)).Execute();
            return DataStatus.Success;
        }
        public static DataStatus DelByBrandId(DataSource ds, int brandid)
        {
            Db<ProductBrandMapping>.Query(ds).Delete().Where(W("BrandId", brandid)).Execute();
            return DataStatus.Success;
        }
    }
}
