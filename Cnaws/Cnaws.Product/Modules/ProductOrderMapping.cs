using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Json;
using System.Collections.Generic;
using Cnaws.Data.Query;
namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductOrderMapping : NoIdentityModule
    {
        [DataColumn(true, 36)]
        public string OrderId = null;
        [DataColumn(true)]
        public long ProductId = 0L;
        /// <summary>
        /// 省
        /// </summary>
        public int Province = 0;
        /// <summary>
        /// 市
        /// </summary>
        public int City = 0;
        /// <summary>
        /// 区
        /// </summary>
        public int County = 0;
        /// <summary>
        /// 单价
        /// </summary>
        public Money Price = 0;
        /// <summary>
        /// 数量
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// 总金额
        /// </summary>
        public Money TotalMoney = 0;
        /// <summary>
        /// 是否评价
        /// </summary>
        public bool Evaluation = false;
        /// <summary>
        /// 产品标题
        /// </summary>
        [DataColumn(128)]
        public string ProductTitle = null;
        /// <summary>
        /// 产品信息
        /// </summary>
        public string ProductInfo = null;
        /// <summary>
        /// 是否申请售后
        /// </summary>
        public bool IsService = false;
        /// <summary>
        /// 售后订单号
        /// </summary>
        [DataColumn(36)]
        public string AfterSalesOrderId = null;
        public ProductOrderMapping()
        {
        }
        public ProductOrderMapping(DataSource ds, string orderId, Product p, int count)
        {
            OrderId = orderId;
            ProductId = p.Id;
            Price = p.GetSalePrice();
            Count = count;
            TotalMoney = Price * Count;
            ProductInfo = BuildInfo(ds, p);
        }
        public ProductOrderMapping(DataSource ds, string orderId, Product p, int count, int province, int city, int county)
        {
            OrderId = orderId;
            ProductId = p.Id;
            Province = province;
            City = city;
            County = county;
            Price = p.GetSalePrice(ds, province, city, county);
            Count = count;
            TotalMoney = Price * Count;
            ProductInfo = BuildInfo(ds, p);
        }

        public Money GetSalePrice(DataSource ds, int province, int city, int county)
        {
            Product product = Product.GetById(ds, ProductId);
            return product.GetSalePrice(ds, province, city, county);
        }
        public bool GetSaleArea(DataSource ds, int province, int city, int county)
        {
            Product product = Product.GetById(ds, ProductId);
            return product.GetSaleArea(ds, province, city, county);
        }
        public ProductCacheInfo GetProductInfo()
        {
            return JsonValue.Deserialize<ProductCacheInfo>(ProductInfo);
        }

        public static string BuildInfo(DataSource ds, Product p)
        {
            return JsonValue.Serialize(new ProductCacheInfo(ds, p));
        }
        public string GetImage(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] arr = s.Split(Product.ImageSplitChar);
                if (arr.Length > 0)
                    return arr[0];
            }
            return string.Empty;
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ProductId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ProductId", "ProductId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            //Product product = Product.GetById(ds, ProductId);
            //ProductOrderSettlement settlement = new ProductOrderSettlement
            //{
            //    OrderId = OrderId,
            //    ProductId = ProductId,
            //    CostPrice = product.CostPrice,
            //    Settlement = product.Settlement,
            //    RoyaltyRate = product.RoyaltyRate
            //};
            //if (product.Wholesale && product.WholesalePrice > 0)
            //{
            //    settlement.ProductType = EProductType.Wholesale;
            //}
            //else if (product.DiscountState == DiscountState.Activated && product.DiscountBeginTime <DateTime.Now && product.DiscountEndTime> DateTime.Now)
            //{
            //    settlement.ProductType = EProductType.GroupBuy;
            //}
            //else
            //{
            //    settlement.ProductType = EProductType.Routine;
            //}
            ////增加收益快照GetRoyaltyByOrderMapping
            //ProductOrder order = GetOrderById(ds);
            //DataJoin<ProductOrder, ProductOrderMapping> s = new DataJoin<ProductOrder, ProductOrderMapping>(order, this);

            return base.OnInsertBefor(ds, mode, ref columns);
        }
        public ProductOrder GetOrderById(DataSource ds)
        {
            return Db<ProductOrder>.Query(ds).Select().Where(W("Id", OrderId)).First<ProductOrder>();
        }
        public static IList<ProductOrderMapping> GetAllByOrder(DataSource ds, string orderId)
        {
            return DataQuery
                .Select<ProductOrderMapping>(ds)
                .Where(P("OrderId", orderId))
                .ToList<ProductOrderMapping>();
        }
        public static SplitPageData<ProductOrderMapping> GetUnCommentOrder(DataSource ds, long userId, long index, int size)
        {
            long count;
            IList<ProductOrderMapping> list = Db<ProductOrderMapping>.Query(ds)
                 .Select(S<ProductOrderMapping>(), S<ProductOrder>("CreationDate"))
                 .LeftJoin(O<ProductOrderMapping>("OrderId"), O<ProductOrder>("Id"))
                 .Where(W<ProductOrderMapping>("Evaluation", false) & W<ProductOrder>("UserId", userId))
                 .OrderBy(D<ProductOrder>("CreationDate"))
                 .ToList<ProductOrderMapping>(size, index, out count);

            return new SplitPageData<ProductOrderMapping>(index, size, list, count);
        }

        public static ProductOrderMapping GetById(DataSource ds, string orderId, long productId)
        {
            return Db<ProductOrderMapping>.Query(ds)
                .Select()
                .Where(W("OrderId", orderId) & W("ProductId", productId))
                .First<ProductOrderMapping>();
        }

        public static DataStatus ModByArea(DataSource ds, ProductOrderMapping pom)
        {
            if (Db<ProductOrderMapping>.Query(ds).Update()
                .Set("Province", pom.Province)
                .Set("City", pom.City)
                .Set("County", pom.County)
                .Set("Price", pom.Price)
                .Where(W("OrderId", pom.OrderId) & W("ProductId", pom.ProductId)).Execute() > 0
                )
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

        public static SplitPageData<DataJoin<ProductOrderMapping, ProductOrder>> GetServiceList(DataSource ds, long userId, long pageIndex, int size)
        {
            long count;
            IList<DataJoin<ProductOrderMapping, ProductOrder>> list = Db<ProductOrderMapping>.Query(ds)
                .Select(S<ProductOrderMapping>(), S<ProductOrder>("CreationDate"))
                .LeftJoin(O<ProductOrderMapping>("OrderId"), O<ProductOrder>("Id"))
                .Where(W<ProductOrder>("UserId", userId) & W<ProductOrder>("State", OrderState.Finished))
                .OrderBy(D<ProductOrder>("CreationDate"))
                .ToList<DataJoin<ProductOrderMapping, ProductOrder>>(size, pageIndex, out count);
            return new SplitPageData<DataJoin<ProductOrderMapping, ProductOrder>>(pageIndex, size, list, count);
        }
    }
}
