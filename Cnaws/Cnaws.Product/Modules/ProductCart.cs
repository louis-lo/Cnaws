using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductCart : LongIdentityModule
    {
        public long UserId = 0L;
        public long SupplierId = 0L;
        public long ProductId = 0L;
        [DataColumn(128)]
        public string Title = null;
        [DataColumn(256)]
        public string Image = null;
        public string Attributes = null;
        public Money Price = 0;
        public Money SalePrice = 0;
        public int Count = 0;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        public ProductCart()
        {

        }
        public ProductCart(DataSource ds, long userId, Product value, int count)
        {
            UserId = userId;
            ProductId = value.Id;
            Title = value.Title;
            try { Image = value.GetImages()[0]; }
            catch (Exception) { }
            Attributes = value.GetAttributes(ds);
            Price = value.Price;
            SalePrice = value.GetSalePrice();
            Count = count;
            SupplierId = value.SupplierId;
            CreationDate = DateTime.Now;
        }
        public ProductCart(DataSource ds, long userId, Product value, int count, int province, int city, int county)
        {
            UserId = userId;
            ProductId = value.Id;
            Title = value.Title;
            try { Image = value.GetImages()[0]; }
            catch (Exception) { }
            Attributes = value.GetAttributes(ds);
            Price = value.GetPrice(ds, province, city, county);//根据地区显示价格
            SalePrice = value.GetSalePrice(ds, province, city, county);//根据地区显示销售价
            Count = count;
            SupplierId = value.SupplierId;
            CreationDate = DateTime.Now;
        }
        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
            DropIndex(ds, "ProductId");
            DropIndex(ds, "UserIdProductId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
            CreateIndex(ds, "ProductId", "ProductId");
            CreateIndex(ds, "UserIdProductId", "UserId", "ProductId");
        }

        private void Load(DataSource ds, Product p)
        {
            Title = p.Title;
            try { Image = p.GetImages()[0]; }
            catch (Exception) { }
            Attributes = p.GetAttributes(ds);
            Price = p.Price;
            SalePrice = p.GetSalePrice();
        }

        private static dynamic LoadDynamic(DataSource ds, dynamic p)
        {
            p.ProductCart_Title = p.Product_Title;
            try { p.ProductCart_Image = p.Product_Image.Split('|')[0]; }
            catch (Exception) { }
            p.ProductCart_Attributes = ProductMapping.GetAttributes(ds, p.Product_Id);

            if (!(p.ProductAreaMapping_Price is DBNull) && p.ProductAreaMapping_Price > 0)
                p.ProductCart_Price = p.ProductAreaMapping_Price;
            else
                p.ProductCart_Price = p.Product_Price;
            DateTime now = DateTime.Now;
            if(p.Product_DiscountState == (int)DiscountState.Activated && (now >= p.Product_DiscountBeginTime && now < p.Product_DiscountEndTime))
            {
                p.ProductCart_SalePrice = p.Product_DiscountPrice;
            }
            else if (p.Product_Wholesale)
                p.ProductCart_SalePrice = p.Product_WholesalePrice;
            else if (!(p.ProductAreaMapping_Price is DBNull) && p.ProductAreaMapping_Price > 0)
                p.ProductCart_SalePrice = p.ProductAreaMapping_Price;
            else
                p.ProductCart_SalePrice = p.Product_Price;
            return p;
        }

        public Money GetTotalMoney()
        {
            return Price * Count;
        }

        public virtual DataStatus Add(DataSource ds)
        {
            if (ExecuteCount<ProductCart>(ds, P("ProductId", ProductId) & P("UserId", UserId)) > 0)
                return DataStatus.ExistOther;
            return Insert(ds);
        }
        public DataStatus Remove(DataSource ds)
        {
            return Delete(ds, "UserId", "Id");
        }
        public static DataStatus Remove(DataSource ds, long productId, long userId)
        {
            return (new ProductCart() { ProductId = productId, UserId = userId }).Delete(ds, "ProductId", "UserId");
        }

        public static ProductCart GetProductByUser(DataSource ds, long userid, long productid)
        {
            return Db<ProductCart>.Query(ds).Select().Where(W("UserId", userid) & W("ProductId", productid)).First<ProductCart>();
        }

        public static ProductCart GetById(DataSource ds, long id)
        {
            return ExecuteSingleRow<ProductCart>(ds, P("Id", id));
        }
        public static long[] GetBySupplierId(DataSource ds, long userId)
        {
            return Db<ProductCart>.Query(ds).Select(S<ProductCart>("SupplierId")).InnerJoin(O<ProductCart>("SupplierId"),O<Supplier>("UserId")).Where(W<ProductCart>("UserId", userId)).GroupBy(G<ProductCart>("SupplierId")).ToArray<long>();
        }

        public static long[] GetByDistributorId(DataSource ds, long userId)
        {
            return Db<ProductCart>.Query(ds).Select(S<ProductCart>("SupplierId")).InnerJoin(O<ProductCart>("SupplierId"), O<Distributor>("UserId")).Where(W<ProductCart>("UserId", userId)).GroupBy(G<ProductCart>("SupplierId")).ToArray<long>();
        }
        public static long GetCountByUser(DataSource ds, long userId)
        {
            //return ExecuteCount<ProductCart>(ds, P("UserId", userId));
            IList<DataJoin<ProductCart, Product>> list;
            list = Db<ProductCart>.Query(ds).Select(S<ProductCart>("Id"), S<Product>("Id"), S<Product>("State")).InnerJoin(O<ProductCart>("ProductId"), O<Product>("Id")).Where(W("UserId", userId)).ToList<DataJoin<ProductCart, Product>>();
            long Total=0;
            foreach (DataJoin<ProductCart, Product> item in list)
            {
                if (item.B.IsPublish())
                    Total++;
            }
            return Total;
        }
        public static IList<DataJoin<ProductCart, Product>> GetPageByUser(DataSource ds, long userId)
        {
            IList<DataJoin<ProductCart, Product>> list;
            list=Db<ProductCart>.Query(ds).Select(S<ProductCart>(), S<Product>()).InnerJoin(O<ProductCart>("ProductId"), O<Product>("Id")).Where(W("UserId", userId)).ToList<DataJoin<ProductCart, Product>>();

            foreach (DataJoin<ProductCart, Product> item in list)
            {
                if (item.B.IsPublish())
                    item.A.Load(ds, item.B);
            }
            return list;
        }
        /// <summary>
        /// 根据用户以及省市区获取购物车列表
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <returns></returns>
        public static IList<dynamic> GetPageByUser(DataSource ds, long userId, int province, int city, int county)
        {
            IList<dynamic> list;
            DbWhereQueue where = W<ProductCart>("UserId", userId) & (W<Product>("State", ProductState.Sale) | W<Product>("State", ProductState.BeforeSaved));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere &= W<ProductAreaMapping>("Province", province);
            areawhere &= W<ProductAreaMapping>("City", city);
            areawhere &= W<ProductAreaMapping>("County", county);
            DbWhereQueue areaSaleswhere = new DbWhereQueue();
            areaSaleswhere &= (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0));
            areaSaleswhere &= (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0));
            areaSaleswhere &= (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0));

            list = Db<ProductCart>.Query(ds).Select(S<ProductCart>(), S_AS<ProductSalesArea>("County", "ProductCart_County"), S<Product>("Id"), S<Product>("SortNum"), S<Product>("Inventory"),S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("ProductType"),S<Product>("State"), S<ProductAreaMapping>())
                .InnerJoin(O<ProductCart>("ProductId"), O<Product>("Id"))
                 .LeftJoin(O<ProductCart>("ProductId"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                 .LeftJoin(O<ProductCart>("ProductId"), O<ProductSalesArea>("ProductId")).Select().Where(areaSaleswhere).Result()
                 .Where(where)
                .OrderBy(D<ProductCart>("CreationDate"))
                .ToList();
            List<dynamic> newlist = new List<dynamic>();
            foreach (dynamic item in list)
            {
                newlist.Add(LoadDynamic(ds, item));
            }
            return newlist;
        }
                
        public static DataStatus RemoveCart(DataSource ds, long id, long userId)
        {
            return (new ProductCart() { Id = id, UserId = userId }).Delete(ds, "Id", "UserId");
        }
    }
}
