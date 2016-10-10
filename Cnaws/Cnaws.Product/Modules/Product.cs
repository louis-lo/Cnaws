using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Json;
using System.Collections;
using System.Collections.Generic;
using Cnaws.Statistic.Modules;
using Cnaws.Data.Query;
using System.Collections.Specialized;

namespace Cnaws.Product.Modules
{
    public enum FreightType
    {
        /// <summary>
        /// 固定价格
        /// </summary>
        Fix = 0,
        /// <summary>
        /// 运费模板
        /// </summary>
        Template = 1
    }
    /// <summary>
    /// 产品状态
    /// </summary>
    public enum ProductState
    {
        /// <summary>
        /// 仓库中
        /// </summary>
        Saved = 0,
        /// <summary>
        /// 出售中
        /// </summary>
        Sale = 1,
        /// <summary>
        /// 准备上架
        /// </summary>
        BeforeSale = 2,
        /// <summary>
        /// 准备下架
        /// </summary>
        BeforeSaved = 3,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 4
    }
    public enum DiscountState
    {
        /// <summary>
        /// 无优惠
        /// </summary>
        None = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        Approval = 1,
        /// <summary>
        /// 已激活
        /// </summary>
        Activated = 2
    }

    public enum ProductOrderBy
    {
        Default = 0,
        /// <summary>
        /// 销量
        /// </summary>
        SaleDesc,
        SaleAsc,
        /// <summary>
        /// 人气
        /// </summary>
        ViewDesc,
        ViewAsc,
        /// <summary>
        /// 价格
        /// </summary>
        PriceAsc,
        PriceDesc
    }


    [Serializable]
    public class Product : LongIdentityModule
    {
        public const char ImageSplitChar = '|';
        public const int ViewType = 1;
        public const int SaleType = 2;

        /// <summary>
        /// 卖家Id
        /// </summary>
        public long SupplierId = 0L;
        /// <summary>
        /// 系列Id
        /// </summary>
        public long ParentId = 0L;
        /// <summary>
        /// 分类Id
        /// </summary>
        public int CategoryId = 0;
        /// <summary>
        /// 品牌Id
        /// </summary>
        public int BrandId = 0;
        /// <summary>
        /// 标题
        /// </summary>
        [DataColumn(128)]
        public string Title = null;
        /// <summary>
        /// 图片，以“|”分割
        /// </summary>
        public string Image = null;
        /// <summary>
        /// 正文
        /// </summary>
        public string Content = null;
        /// <summary>
        /// 关键字
        /// </summary>
        [DataColumn(2000)]
        public string Keywords = null;
        /// <summary>
        /// 说明
        /// </summary>
        [DataColumn(2000)]
        public string Description = null;
        /// <summary>
        /// 条形码
        /// </summary>
        [DataColumn(32)]
        public string BarCode = null;
        /// <summary>
        /// 单位
        /// </summary>
        [DataColumn(8)]
        public string Unit = null;
        /// <summary>
        /// 库存
        /// </summary>
        public int Inventory = 0;
        /// <summary>
        /// 库存预警
        /// </summary>
        public int InventoryAlert = 0;
        /// <summary>
        /// 成本价
        /// </summary>
        public Money CostPrice = 0;
        /// <summary>
        /// 县级成本价
        /// </summary>
        public Money CountyPrice = 0;
        /// <summary>
        /// 网点成本价
        /// </summary>
        public Money DotPrice = 0;
        /// <summary>
        /// 单价
        /// </summary>
        public Money Price = 0;
        /// <summary>
        /// 市场价
        /// </summary>
        public Money MarketPrice = 0;
        /// <summary>
        /// 优惠状态
        /// </summary>
        public DiscountState DiscountState = DiscountState.None;
        /// <summary>
        /// 优惠价
        /// </summary>
        public Money DiscountPrice = 0;
        /// <summary>
        /// 折扣,自动生成
        /// </summary>
        public int Discount = 0;
        /// <summary>
        /// 优惠开始时间
        /// </summary>
        public DateTime DiscountBeginTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 优惠结束时间
        /// </summary>
        public DateTime DiscountEndTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 能否批发
        /// </summary>
        public bool Wholesale = false;
        /// <summary>
        /// 批发价
        /// </summary>
        public Money WholesalePrice = 0;
        /// <summary>
        /// 起批数量
        /// </summary>
        public int WholesaleCount = 0;
        /// <summary>
        /// 折扣,自动生成
        /// </summary>
        public int WholesaleDiscount = 0;
        /// <summary>
        /// 运费类型
        /// </summary>
        public FreightType FreightType = FreightType.Fix;
        /// <summary>
        /// 固定运费价格
        /// </summary>
        public Money FreightMoney = 0;
        public int Province = 00;
        public int City = 0;
        public int County = 0;
        /// <summary>
        /// 运费模板Id
        /// </summary>
        public long FreightTemplate = 0;
        /// <summary>
        /// 是否开增值税发票
        /// </summary>
        public bool HasReceipt = false;
        /// <summary>
        /// 发布类型
        /// </summary>
        public ProductState State = ProductState.Saved;
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime SaleTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        ///// <summary>
        ///// 发布时间
        ///// </summary>
        //public DateTime PublishTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        ///// <summary>
        ///// 能否预约
        ///// </summary>
        //public bool CanReservation = false;
        /// <summary>
        /// 推荐
        /// </summary>
        public bool Recommend = false;
        /// <summary>
        /// 分类精选
        /// </summary>
        public int CategoryBest = 0;
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        ///// <summary>
        ///// 是否审核
        ///// </summary>
        //public bool Approved = false;
        ///// <summary>
        ///// 是否删除
        ///// </summary>
        //public bool IsDel = false;
        public int SortNum = 0;

        /// <summary>
        /// 店铺分类Id
        /// </summary>
        public int StoreCategoryId = 0;
        /// <summary>
        /// 店铺内产品排序
        /// </summary>
        public int StoreSortNum = 0;
        /// <summary>
        /// 店铺内产品推荐
        /// </summary>
        public bool StoreRecommend = false;
        /// <summary>
        /// 店铺内产品分类推荐
        /// </summary>
        public int StoreCategoryBest = 0;

        /// <summary>
        /// 结算方式
        /// </summary>
        public SettlementType Settlement = SettlementType.Fixed;
        /// <summary>
        /// 当类型为提成时的提成率/100
        /// </summary>
        public int RoyaltyRate = 0;

        public int ProductType = 1;//1、城品惠2、乡道馆
        /// <summary>
        /// 缓存产品规格
        /// </summary>
        [DataColumn(128)]
        public string Norms = null;

        /// <summary>
        /// 产品供应商
        /// </summary>
        [DataColumn(30)]
        public string ProductSupplier = null;
        /// <summary>
        /// 重量 
        /// </summary>
        public int Weight = 0;
        /// <summary>
        /// 体积
        /// </summary>
        public int Volume = 0;
        /// <summary>
        /// 供应类型1为本地供应,0为全国优选
        /// </summary>
        public int SupplierType = 0;

        public string[] GetImages()
        {
            if (Image != null)
                return Image.Split(ImageSplitChar);
            return new string[] { };
        }
        public string GetImage()
        {
            string[] imgs = GetImages();
            if (imgs.Length > 0)
                return imgs[0];
            return string.Empty;
        }
        public Money GetFreight(DataSource ds, int provice, int city, Money total)
        {
            if (FreightType == FreightType.Fix)
                return FreightMoney;

            FreightTemplate tmp = Modules.FreightTemplate.GetById(ds, FreightTemplate);
            if (tmp != null)
            {
                FreightMapping map = FreightMapping.GetMapping(ds, tmp.Id, provice, city);
                if (map != null)
                {
                    if (map.StepNumber > 0 && total >= map.StepNumber)
                        return map.StepMoney;
                    return map.Money;
                }
            }

            return 0;
        }
        private string GetFreightStringImpl(Money money)
        {
            if (money == 0)
                return "包邮";
            return money.ToString("C2");
        }
        public string GetFreightString(DataSource ds, int provice, int city, int count = 1)
        {
            if (FreightType == FreightType.Fix)
                return GetFreightStringImpl(FreightMoney);

            FreightTemplate tmp = Modules.FreightTemplate.GetById(ds, FreightTemplate);
            if (tmp != null)
            {
                string Unit = "";
                if (tmp.Type == Modules.FreightTemplate.ValuationType.ThePrece)
                    Unit = "件";
                else if (tmp.Type == Modules.FreightTemplate.ValuationType.Volume)
                    Unit = "m³";
                else if (tmp.Type == Modules.FreightTemplate.ValuationType.Weight)
                    Unit = "Kg";
                FreightMapping map = FreightMapping.GetMapping(ds, tmp.Id, provice, city);
                if (map != null)
                {
                    if (map.StepNumber > 0)
                        return string.Concat("", GetFreightStringImpl(map.Money), "&nbsp;每", GetFreightStringImpl(map.StepNumber), "/", Unit, "&nbsp;", GetFreightStringImpl(map.StepMoney));
                    return GetFreightStringImpl(map.Money);
                }
                else
                {
                    if (tmp.StepNumber > 0)
                        return string.Concat("", GetFreightStringImpl(tmp.Money), "&nbsp;满", GetFreightStringImpl(tmp.StepNumber), "&nbsp;", GetFreightStringImpl(tmp.StepMoney));
                    return GetFreightStringImpl(tmp.Money);
                }
            }

            return GetFreightStringImpl(0);
        }

        public string GetNewFreightString(DataSource ds, int provice, int city, int Count)
        {
            if (ProductType == 1)
            {
                Supplier supplier = Supplier.GetById(ds, SupplierId);
                if (supplier.IsActivityFree)
                {
                    if ((Price * Count) >= supplier.ActivityCondition)
                    {
                        return GetFreightStringImpl(supplier.ActivityFree);
                    }
                }
            }
            if (FreightType == FreightType.Fix)
                return GetFreightStringImpl(FreightMoney);

            FreightTemplate tmp = Modules.FreightTemplate.GetById(ds, FreightTemplate);
            if (tmp != null)
            {
                if (tmp.Number <= 0) tmp.Number = 1;
                FreightMapping map = FreightMapping.GetMapping(ds, tmp.Id, provice, city);
                if (map != null)
                {
                    if (map.Number <= 0) map.Number = 1;
                    if (tmp.Type == Modules.FreightTemplate.ValuationType.ThePrece)
                    {
                        if (Count > map.Number && map.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((Count - map.Number) % map.StepNumber == 0)
                                return GetFreightStringImpl((Count - map.Number) / map.StepNumber * map.StepMoney + map.Money);
                            else
                                return GetFreightStringImpl((((Count - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money);
                        }
                        else
                            return GetFreightStringImpl(map.Money);
                    }
                    else if (tmp.Type == Modules.FreightTemplate.ValuationType.Volume)
                    {
                        int FVolumn = Count * Volume;
                        if (FVolumn > map.Number && map.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((FVolumn - map.Number) % map.StepNumber == 0)
                                return GetFreightStringImpl((FVolumn - map.Number) / map.StepNumber * map.StepMoney + map.Money);
                            else
                                return GetFreightStringImpl((((FVolumn - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money);
                        }
                        else
                            return GetFreightStringImpl(map.Money);
                    }
                    else if (tmp.Type == Modules.FreightTemplate.ValuationType.Weight)
                    {
                        int FVolumn = Count * Weight;
                        if (FVolumn > map.Number && map.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((FVolumn - map.Number) % map.StepNumber == 0)
                                return GetFreightStringImpl((FVolumn - map.Number) / map.StepNumber * map.StepMoney + map.Money);
                            else
                                return GetFreightStringImpl((((FVolumn - map.Number) / map.StepNumber) + 1) * map.StepMoney + map.Money);
                        }
                        else
                            return GetFreightStringImpl(map.Money);
                    }
                }
                else
                {
                    if (tmp.Type == Modules.FreightTemplate.ValuationType.ThePrece)
                    {
                        if (Count > tmp.Number && tmp.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((Count - tmp.Number) % tmp.StepNumber == 0)
                                return GetFreightStringImpl((Count - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money);
                            else
                                return GetFreightStringImpl((((Count - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money);
                        }
                        else
                            return GetFreightStringImpl(tmp.Money);
                    }
                    else if (tmp.Type == Modules.FreightTemplate.ValuationType.Volume)
                    {
                        int FVolumn = Count * Volume;
                        if (FVolumn > tmp.Number && tmp.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((FVolumn - tmp.Number) % tmp.StepNumber == 0)
                                return GetFreightStringImpl((FVolumn - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money);
                            else
                                return GetFreightStringImpl((((FVolumn - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money);
                        }
                        else
                            return GetFreightStringImpl(tmp.Money);
                    }
                    else if (tmp.Type == Modules.FreightTemplate.ValuationType.Weight)
                    {
                        int FVolumn = Count * Weight;
                        if (FVolumn > tmp.Number && tmp.StepMoney > 0 && map.StepNumber > 0)
                        {
                            if ((FVolumn - tmp.Number) % tmp.StepNumber == 0)
                                return GetFreightStringImpl((FVolumn - tmp.Number) / tmp.StepNumber * tmp.StepMoney + tmp.Money);
                            else
                                return GetFreightStringImpl((((FVolumn - tmp.Number) / tmp.StepNumber) + 1) * tmp.StepMoney + tmp.Money);
                        }
                        else
                            return GetFreightStringImpl(tmp.Money);
                    }
                }
            }
            return GetFreightStringImpl(0);
        }
        public bool IsPublish()
        {
            //if (PublishType == PublishType.Sale || (PublishType == PublishType.OnTime && DateTime.Now >= PublishTime))
            //    return true;
            //return false;
            return State == ProductState.Sale;
        }
        public string GetAttributes(DataSource ds)
        {
            return JsonValue.Serialize(ProductMapping.GetAllByProduct(ds, Id));
        }

        //public Money GetFreight(DataSource ds)
        //{
        //    if (FreightType == FreightType.Fix)
        //        return FreightMoney;
        //    throw new NotSupportedException();
        //}
        public bool IsDiscount()
        {
            if (DiscountState == DiscountState.Activated)
            {
                DateTime now = DateTime.Now;
                return now >= DiscountBeginTime && now < DiscountEndTime;
            }
            return false;
        }
        /// <summary>
        /// 返回产品的销售地
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public IList<ProductAreaMapping> GetSaleArea(DataSource ds)
        {
            return Db<ProductAreaMapping>.Query(ds)
                .Select()
                .Where(W("ProductId", Id))
                .ToList<ProductAreaMapping>();
        }

        public bool GetSaleArea(DataSource ds, int province, int city, int county)
        {
            return Db<ProductSalesArea>.Query(ds)
                .Select()
                .Where(W("ProductId", Id) & (W("Province", province) | W("Province", 0)) & (W("City", city) | W("City", 0)) & (W("County", county) | W("County", 0)))
                .First<ProductSalesArea>() != null;
        }

        public string GetDiscount()
        {
            if ((Discount % 10) != 0)
                return (Discount / 10.0).ToString("F1");
            return (Discount / 10).ToString();
        }
        public Countdown GetDiscountCountdown()
        {
            DateTime now = DateTime.Now;
            if (DiscountEndTime > now)
                return DiscountEndTime - now;
            return new Countdown();
        }
        public Money GetSalePrice()
        {
            if (IsDiscount())
                return DiscountPrice;
            if (Wholesale)
                return WholesalePrice;
            return Price;
        }
        public Money GetPrice(DataSource ds, int province, int city, int county)
        {
            ProductAreaMapping areamapping = Db<ProductAreaMapping>.Query(ds).Select().Where(W("ProductId", Id) & W("Province", province) & W("City", city) & W("County", county)).First<ProductAreaMapping>();
            if (areamapping != null && areamapping.Price > 0)
                return areamapping.Price;
            return Price;

        }
        public Money GetSalePrice(DataSource ds, int province, int city, int county)
        {
            if (IsDiscount())
                return DiscountPrice;
            if (Wholesale)
                return WholesalePrice;
            else
            {
                ProductAreaMapping areamapping = Db<ProductAreaMapping>.Query(ds).Select().Where(W("ProductId", Id) & W("Province", province) & W("City", city) & W("County", county)).First<ProductAreaMapping>();
                if (areamapping != null && areamapping.Price > 0)
                    return areamapping.Price;
                return Price;
            }
        }
        public Money GetTotalMoney(int count)
        {
            if (IsPublish())
                return GetSalePrice() * count;
            return 0;
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "CategoryId");
            DropIndex(ds, "BrandId");
            DropIndex(ds, "State");
            DropIndex(ds, "SupplierId");
            DropIndex(ds, "SupplierIdState");
            DropIndex(ds, "RecommendState");
            DropIndex(ds, "CategoryIdState");
            DropIndex(ds, "CategoryBestState");
            DropIndex(ds, "DiscountStateState");
            DropIndex(ds, "CategoryIdRecommendState");

            DropIndex(ds, "CategoryIdParentId");
            DropIndex(ds, "BrandIdParentId");
            DropIndex(ds, "StateParentId");
            DropIndex(ds, "SupplierIdParentId");
            DropIndex(ds, "SupplierIdStateParentId");
            DropIndex(ds, "RecommendStateParentId");
            DropIndex(ds, "CategoryIdStateParentId");
            DropIndex(ds, "CategoryBestStateParentId");
            DropIndex(ds, "DiscountStateStateParentId");
            DropIndex(ds, "CategoryIdRecommendStateParentId");

            DropIndex(ds, "StateWholesale");
            DropIndex(ds, "CategoryIdStateWholesale");

            DropIndex(ds, "StateWholesaleParentId");
            DropIndex(ds, "CategoryIdStateWholesaleParentId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "CategoryId", "CategoryId");
            CreateIndex(ds, "BrandId", "BrandId");
            CreateIndex(ds, "State", "State");
            CreateIndex(ds, "SupplierId", "SupplierId");
            CreateIndex(ds, "SupplierIdState", "SupplierId", "State");
            CreateIndex(ds, "RecommendState", "Recommend", "State");
            CreateIndex(ds, "CategoryIdState", "CategoryId", "State");
            CreateIndex(ds, "CategoryBestState", "CategoryBest", "State");
            CreateIndex(ds, "DiscountStateState", "DiscountState", "State");
            CreateIndex(ds, "CategoryIdRecommendState", "CategoryId", "Recommend", "State");

            CreateIndex(ds, "CategoryIdParentId", "CategoryId", "ParentId");
            CreateIndex(ds, "BrandIdParentId", "BrandId", "ParentId");
            CreateIndex(ds, "StateParentId", "State", "ParentId");
            CreateIndex(ds, "SupplierIdParentId", "SupplierId", "ParentId");
            CreateIndex(ds, "SupplierIdStateParentId", "SupplierId", "State", "ParentId");
            CreateIndex(ds, "RecommendStateParentId", "Recommend", "State", "ParentId");
            CreateIndex(ds, "CategoryIdStateParentId", "CategoryId", "State", "ParentId");
            CreateIndex(ds, "CategoryBestStateParentId", "CategoryBest", "State", "ParentId");
            CreateIndex(ds, "DiscountStateStateParentId", "DiscountState", "State", "ParentId");
            CreateIndex(ds, "CategoryIdRecommendStateParentId", "CategoryId", "Recommend", "State", "ParentId");

            CreateIndex(ds, "StateWholesale", "State", "Wholesale");
            CreateIndex(ds, "CategoryIdStateWholesale", "CategoryId", "State", "Wholesale");

            CreateIndex(ds, "StateWholesaleParentId", "State", "Wholesale", "ParentId");
            CreateIndex(ds, "CategoryIdStateWholesaleParentId", "CategoryId", "State", "Wholesale", "ParentId");
        }
        /// <summary>
        /// 销售状态条件
        /// </summary>
        /// <returns></returns>
        public static DbWhereQueue GetStateWhereQueue()
        {
            return (W("State", ProductState.Sale) | W("State", ProductState.BeforeSaved));
        }
        /// <summary>
        /// 销售状态条件
        /// </summary>
        /// <returns></returns>
        protected static DbWhereQueue GetStateWhereQueue<T>() where T : Product
        {
            return (W<T>("State", ProductState.Sale) | W<T>("State", ProductState.BeforeSaved));
        }
        /// <summary>
        /// 地区条件
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <returns></returns>
        protected static DbWhereQueue GetAreaMappingWhereQueue(int province, int city, int county)
        {
            DbWhereQueue where = W<ProductAreaMapping>("Province", province);
            where &= W<ProductAreaMapping>("City", city);
            where &= W<ProductAreaMapping>("County", county);
            where &= W<ProductAreaMapping>("Saled", true);
            return where;
        }
        /// <summary>
        /// 父级为0的条件
        /// </summary>
        /// <returns></returns>
        public static DbWhereQueue GetParentWhereQueue()
        {
            return W("ParentId", 0);
        }
        /// <summary>
        /// 父级为0的条件
        /// </summary>
        /// <returns></returns>
        protected static DbWhereQueue GetParentWhereQueue<T>() where T : Product
        {
            return W<T>("ParentId", 0);
        }

        public string GetAttribute(Dictionary<long, string> dict, long id)
        {
            string value;
            if (dict.TryGetValue(id, out value))
                return value;
            return string.Empty;
        }
        /// <summary>
        /// 获取产品属性名称
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public IList<ProductSerie> GetSerie(DataSource ds)
        {
            return ProductSerie.GetAll(ds, Id);
        }
        /// <summary>
        /// 获取产品属性值
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="serieId"></param>
        /// <returns></returns>
        public ProductMapping GetMapping(DataSource ds, long serieId)
        {
            return ProductMapping.GetBySerie(ds, Id, serieId);
        }
        /// <summary>
        /// 获取当前产品所有没有删除的子产品
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public IList<Product> GetChildren(DataSource ds)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(W("ParentId", Id) & W("State", ProductState.Deleted, DbWhereType.NotEqual))
                .ToList<Product>();
        }
        /// <summary>
        /// 删除产品到回收站
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataStatus RemoveToRecycleBin(DataSource ds, long id)
        {
            ds.Begin();
            try
            {
                if (Db<Product>.Query(ds).Update().Set("State", ProductState.Deleted).Where(W("Id", id)).Execute() == 1)
                {
                    Db<ProductMapping>.Query(ds).Delete().Where(W("ProductId", id)).Execute();
                    ds.Commit();
                    return DataStatus.Success;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                ds.Rollback();
                return DataStatus.Exist;
            }
        }

        /// <summary>
        /// 增加子产品
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentId">主产品Id</param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static DataStatus CreateCopy(DataSource ds, long parentId,out long productId)
        {
            ds.Begin();
            try
            {
                Product value = GetById(ds, parentId);
                if (value == null)
                    throw new Exception();
                value.ParentId = parentId;
                if (value.Insert(ds) != DataStatus.Success)
                    throw new Exception();
                productId = value.Id;
                IList<ProductAttributeMapping> maps = ProductAttributeMapping.GetListByProduct(ds, parentId);
                foreach (ProductAttributeMapping map in maps)
                {
                    map.ProductId = value.Id;
                    if (map.Insert(ds) != DataStatus.Success)
                        throw new Exception();
                }
                IList<ProductSalesArea> salesareas = ProductSalesArea.GetById(ds, parentId);
                foreach (ProductSalesArea area in salesareas)
                {
                    area.ProductId = value.Id;
                    if (area.Insert(ds) != DataStatus.Success)
                        throw new Exception();
                }
                ds.Commit();
                return DataStatus.Success;
            }
            catch (Exception)
            {
                ds.Rollback();
                productId = 0;
                return DataStatus.Failed;
            }
        }
        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataStatus DeleteChild(DataSource ds, long id)
        {
            return (new Product() { Id = id }).Delete(ds);
        }

        public static Product GetById(DataSource ds, long id)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .First<Product>();
        }
        public static Product GetBySupplier(DataSource ds, long id, long supplierId)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(W("SupplierId", supplierId) & W("Id", id))
                .First<Product>();
        }
        public static Product GetSaleProduct(DataSource ds, long id)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(GetStateWhereQueue() & W("Id", id))
                .First<Product>();
        }
        /// <summary>
        /// 根据省市区和Id获取销售的产品
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <returns></returns>
        public static new Product GetSaleProduct(DataSource ds, long id, int province, int city, int county)
        {
            return Db<Product>.Query(ds)
                .Select(S<Product>())
                .InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId"))
                .Where(GetStateWhereQueue<Product>()
                & W<Product>("Id", id) & (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0))
                & (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0))
                & (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0))
                )
                .First<Product>();
        }
        public static Product GetViewProduct(DataSource ds, long id, long supplierId)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where((W("SupplierId", supplierId) | W("SupplierId").InSelect<Supplier>(S("UserId")).Where(W("Subjection", supplierId)).Result()) & W("Id", id))
                .First<Product>();
        }
        public static long GetCountByCategoryId(DataSource ds, int categoryId, int productType = 1)
        {
            return ExecuteCount<Product>(ds, P("CategoryId", categoryId) & P("ProductType", productType));
        }
        public static long GetCountByBrandId(DataSource ds, int brandId, int productType = 1)
        {
            return ExecuteCount<Product>(ds, P("BrandId", brandId) & P("ProductType", productType));
        }

        public static IList<Product> GetTopRecommend(DataSource ds, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(GetStateWhereQueue() & W("Recommend", true) & W("ParentId", 0) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList<Product>(count);
        }

        public static IList<DataJoin<Product, ProductAreaMapping>> GetTopRecommendByArea(DataSource ds, int count, int province, int city, int county, int productType = 1)
        {
            DbWhereQueue where = new DbWhereQueue();
            where = GetStateWhereQueue<Product>() & W<Product>("Recommend", true) & W<Product>("ParentId", 0) & W<Product>("ProductType", productType);
            where &= (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0));
            where &= (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0));
            where &= (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere = GetAreaMappingWhereQueue(province, city, county);
            return Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("CountyPrice"), S<Product>("DotPrice"), S<Product>("MarketPrice"), S<Product>("ProductType"), S<Product>("SupplierType"), S<ProductAreaMapping>("CostPrice"), S<ProductAreaMapping>("CountyPrice"), S<ProductAreaMapping>("DotPrice"), S<ProductAreaMapping>("Price"))
                .LeftJoin(O<Product>("Id"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                .InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId"))
                .Where(where)
                .OrderBy(D<Product>("SortNum"), D<Product>("SaleTime"), D<Product>("Id"))
                .ToList<DataJoin<Product, ProductAreaMapping>>(count);
        }

        public static SplitPageData<Product> GetPageTopRecommend(DataSource ds, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(GetStateWhereQueue() & W("Recommend", true) & W("ParentId", 0) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }
        public static IList<Product> GetTopRecommendByCategory(DataSource ds, int count, int categoryId = 0, int productType = 1)
        {
            DbWhereQueue where = GetStateWhereQueue();
            if (categoryId > 0)
                where &= W("CategoryId", categoryId) & W("ParentId", 0);
            where &= W("Recommend", true);
            where &= W("ProductType", productType);
            return Db<Product>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList<Product>(count);
        }
        public static SplitPageData<DataJoin<Product, StatisticData>> GetPageByArguments(DataSource ds, int categoryId, FilterParameters parameters, int categorylevel, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();
            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}
            ///三级分类
            if (categorylevel == 3)
                where &= W<Product>("CategoryId", categoryId);
            else if (categorylevel == 2)
                where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
            else if (categorylevel == 1)
                where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());

            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SupplierType"));
            orders.Add(D("SortNum"));
            orders.Add(D("SaleTime"));
            orders.Add(D("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D("Price"));
                    break;
            }
            where &= W<Product>("ProductType", productType);
            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"), S_AS<Supplier>("SupplierType", "Level"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList<DataJoin<Product, StatisticData>>(size, parameters.Page, out count);

            return new SplitPageData<DataJoin<Product, StatisticData>>(parameters.Page, size, list, count, show);
        }

        public static SplitPageData<DataJoin<Product, StatisticData>> GetBrandPageByArguments(DataSource ds, int categoryId, bool isbrand, FilterParameters parameters, int categorylevel, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();
            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}
            ///三级分类
            if (categorylevel == 3)
                where &= W<Product>("CategoryId", categoryId);
            else if (categorylevel == 2)
                where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
            else if (categorylevel == 1)
                where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            if (isbrand)
                where &= W<Product>("BrandId", 0, DbWhereType.NotEqual);
            else
                where &= W<Product>("BrandId", 0, DbWhereType.Equal);


            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D("SortNum"));
            orders.Add(D("SaleTime"));
            orders.Add(D("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D("Price"));
                    break;
            }
            where &= W<Product>("ProductType", productType);
            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"), S_AS<Supplier>("SupplierType", "Level"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList<DataJoin<Product, StatisticData>>(size, parameters.Page, out count);

            return new SplitPageData<DataJoin<Product, StatisticData>>(parameters.Page, size, list, count, show);
        }
        public static SplitPageData<DataJoin<Product, StatisticData>> GetPageByDiscount(DataSource ds, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>(), S<StatisticData>("Count"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId"))
                .Where(GetStateWhereQueue<Product>() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("ProductType", productType))
                .OrderBy(A<Product>("DiscountEndTime"), D<Product>("Id"))
                .ToList<DataJoin<Product, StatisticData>>(size, index, out count);
            return new SplitPageData<DataJoin<Product, StatisticData>>(index, size, list, count, show);
        }
        public static SplitPageData<DataJoin<Product, StatisticData>> GetPageBySearch(DataSource ds, NameValueCollection request, FilterParameters parameters, long index, int size, int show = 8)
        {
            long count;
            string[] qs;

            DbWhereQueue where = null;
            string q = request["q"];

            if (string.IsNullOrEmpty(q))
                qs = new string[] { string.Empty };
            else
                qs = q.Split(' ');

            foreach (string s in qs)
            {
                if (where == null)
                    where = W<Product>("Title", s, DbWhereType.Like);
                else
                    where |= W<Product>("Title", s, DbWhereType.Like);
            }
            if (!string.IsNullOrEmpty(request["searchType"]) && request["searchType"] == "搜本店")
            {
                where = (where) & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("SupplierId", request["id"]) & W<Product>("ProductType", 1);
            }
            else if (!string.IsNullOrEmpty(request["searchType"]) && request["searchType"] == "搜特产")
            {
                where = (where) & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("ProductType", 2);
            }
            else if (!string.IsNullOrEmpty(request["searchType"]) && request["searchType"] == "搜本馆")
            {
                where = (where) & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("SupplierId", request["id"]) & W<Product>("ProductType", 2);
            }
            else
            {
                where = (where) & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("ProductType", 1);
            }
            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}

            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SupplierType"));
            orders.Add(D("SortNum"));
            orders.Add(D("SaleTime"));
            orders.Add(D("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D("Price"));
                    break;
            }

            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"), S_AS<Supplier>("SupplierType", "Level"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList<DataJoin<Product, StatisticData>>(size, parameters.Page, out count);

            if (list != null && list.Count > 0)
                StatisticTag.Add(ds, qs);

            return new SplitPageData<DataJoin<Product, StatisticData>>(parameters.Page, size, list, count, show);
        }

        [Obsolete]
        public static SplitPageData<dynamic> ApiGetPageBySearch(DataSource ds, string q, FilterParameters parameters, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            string[] qs;

            DbWhereQueue where = null;
            if (string.IsNullOrEmpty(q))
                qs = new string[] { string.Empty };
            else
                qs = q.Split(' ');

            foreach (string s in qs)
            {
                if (where == null)
                    where = W<Product>("Title", s, DbWhereType.Like);
                else
                    where |= W<Product>("Title", s, DbWhereType.Like);
            }
            where = where & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();

            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}

            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SupplierType"));
            orders.Add(D<Product>("SortNum"));
            orders.Add(D<Product>("SaleTime"));
            orders.Add(D<Product>("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A<Product>("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D<Product>("Price"));
                    break;
            }
            where &= W<Product>("ProductType", productType);
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("ProductType"), S<Product>("SupplierType"), S<StatisticData>("Count"), S<StatisticData>("Type"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList(size, parameters.Page, out count);

            if (list != null && list.Count > 0)
                StatisticTag.Add(ds, qs);

            return new SplitPageData<dynamic>(parameters.Page, size, list, count, show);
        }
        //public static IList<Product> GetTopDiscount(DataSource ds, int count, int categoryId)
        //{
        //    return ds.ExecuteReader<Product>("SELECT TOP(@Top) * FROM [Product] WHERE [CategoryId] IN (SELECT [Id] FROM [ProductCategory] AS P WHERE [ParentId] IN (SELECT [Id] FROM [ProductCategory] AS C WHERE [ParentId]=@Id)) AND [State]=1 ORDER BY [Id] DESC", P("Top", count), P("Id", categoryId));
        //}
        public static IList<Product> GetTopDiscount(DataSource ds, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(GetStateWhereQueue() & W("DiscountState", DiscountState.Activated) & W("ProductType", productType))
                .OrderBy(A("DiscountEndTime"), D("Id"))
                .ToList<Product>(count);
        }
        public static IList<Product> GetTopDiscountEx(DataSource ds, int count, int week, int productType = 1)
        {
            DateTime now = DateTime.Now.AddDays(7 * week);
            return Db<Product>.Query(ds)
                .Select()
                .Where(W("DiscountBeginTime", now, DbWhereType.LessThanOrEqual) & W("DiscountEndTime", now, DbWhereType.GreaterThan) & GetStateWhereQueue() & W("DiscountState", DiscountState.Activated) & W("ParentId", 0) & W("ProductType", productType))
                .OrderBy(A("DiscountEndTime"), D("Id"))
                .ToList<Product>(count);
        }
        public static IList<Product> GetTopDiscountByDay(DataSource ds, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(W("Type", SaleType)).Result()
                .Where(GetStateWhereQueue<Product>() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("ParentId", 0) & W<Product>("ProductType", productType))
                .OrderBy(D("Day"), D("Count"), D("Year"), D("Month"), D("Week"))
                .ToList<Product>(count);
        }
        public static IList<Product> GetTopDiscountByWeek(DataSource ds, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(W("Type", SaleType)).Result()
                .Where(GetStateWhereQueue<Product>() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("ParentId", 0) & W<Product>("ProductType", productType))
                .OrderBy(D("Week"), D("Count"), D("Year"), D("Month"), D("Day"))
                .ToList<Product>(count);
        }
        public static IList<Product> GetTopBestByCategory(DataSource ds, int count, int categoryId, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(GetStateWhereQueue() & W("CategoryBest", categoryId) & W("ParentId", 0) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList<Product>(count);
        }
        public static IList<DataJoin<Product, ProductAreaMapping>> GetTopBestByCategoryByArea(DataSource ds, int count, int categoryId, int province, int city, int county, int productType = 1)
        {
            DbWhereQueue where = new DbWhereQueue();
            where = GetStateWhereQueue<Product>() & W<Product>("CategoryBest", categoryId) & W<Product>("ParentId", 0) & W<Product>("ProductType", productType);
            where &= (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0));
            where &= (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0));
            where &= (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere = GetAreaMappingWhereQueue(province, city, county);
            return Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("CountyPrice"), S<Product>("DotPrice"), S<Product>("MarketPrice"), S<Product>("ProductType"), S<Product>("SupplierType"), S<ProductAreaMapping>("CostPrice"), S<ProductAreaMapping>("CountyPrice"), S<ProductAreaMapping>("DotPrice"), S<ProductAreaMapping>("Price"))
                .LeftJoin(O<Product>("Id"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                .InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId"))
                .Where(where)
                .OrderBy(D<Product>("SortNum"), D<Product>("SaleTime"), D<Product>("Id"))
                .ToList<DataJoin<Product, ProductAreaMapping>>(count);
        }
        public static IList<Product> GetTopHotByMonth(DataSource ds, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(W("Type", SaleType)).Result()
                .Where(GetStateWhereQueue<Product>() & W<Product>("ParentId", 0) & W<Product>("ProductType", productType))
                .OrderBy(D("Month"), D("Count"), D("Year"), D("Week"), D("Day"))
                .ToList<Product>(count);
        }
        public static SplitPageData<Product> GetMonthHotPage(DataSource ds, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(W("Type", SaleType)).Result()
                .Where(GetStateWhereQueue<Product>() & W<Product>("ParentId", 0) & W<Product>("ProductType", productType))
                .OrderBy(D("Month"), D("Count"), D("Year"), D("Week"), D("Day"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }
        public static SplitPageData<dynamic> GetMonthHotPageApi(DataSource ds, long index, int size, int suppliertype, int show = 8, int productType = 1)
        {
            long count;
            DbWhereQueue where = GetStateWhereQueue<Product>() & W<Product>("ProductType", productType);
            if (suppliertype > -1)
            {
                where &= W<Product>("SupplierType", suppliertype);
            }
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("ProductType"), S<Product>("SupplierType"), S<StatisticData>("*"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(W("Type", SaleType)).Result()
                .Where(where)
                .OrderBy(D("Month"), D("Count"), D("Year"), D("Week"), D("Day"))
                .ToList(size, index, out count);
            return new SplitPageData<dynamic>(index, size, list, count, show);
        }
        public static SplitPageData<Product> GetBySupplierState(DataSource ds, long supplierId, ProductState state, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(W("State", state) & W("SupplierId", supplierId) & W("ParentId", 0) & W("ProductType", productType))
                .OrderBy(D("CreationDate"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }
        public static SplitPageData<Product> GetBySupplierStateOrState(DataSource ds, long supplierId, string keyword, ProductState state1, ProductState state2, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            DbWhereQueue where = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                foreach (string s in keyword.Split(' '))
                {
                    if (where == null)
                        where = W("Title", s, DbWhereType.Like);
                    else
                        where |= W("Title", s, DbWhereType.Like);
                }
            }
            else
            {
                where = W("Title", keyword, DbWhereType.Like);
            }
            where = (where) & (W("State", state1) | W("State", state2)) & W("SupplierId", supplierId) & W("ParentId", 0) & W("ProductType", productType);

            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(D("CreationDate"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }
        public static SplitPageData<Product> GetBySupplierStateOrStateParent(DataSource ds, long supplierId, ProductState state1, ProductState state2, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(GetParentWhereQueue() & (W("State", state1) | W("State", state2)) & W("SupplierId", supplierId) & W("ProductType", productType))
                .OrderBy(D("CreationDate"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }

        public static FilterData<DataJoin<Product, StatisticData>> GetPageByParameters(DataSource ds, FilterParameters parameters, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = new DbWhereQueue();
            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}

            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SupplierType"));
            orders.Add(D("SortNum"));
            orders.Add(D("SaleTime"));
            orders.Add(D("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    where &= W("Type", SaleType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    where &= W("Type", SaleType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    where &= W("Type", ViewType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    where &= W("Type", ViewType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D("Price"));
                    break;
            }

            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"), S_AS<Supplier>("SupplierType", "Level"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(where).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("ProductType", productType))
                .OrderBy(orders.ToArray())
                .ToList<DataJoin<Product, StatisticData>>(size, parameters.Page, out count);

            return new FilterData<DataJoin<Product, StatisticData>>(parameters, new SplitPageData<DataJoin<Product, StatisticData>>(parameters.Page, size, list, count, show));
        }

        public static DataStatus ModfiyImageByIds(DataSource ds, long[] ids,string image)
        {
            try {
                if (Db<Product>.Query(ds).Update().Set("Image", image).Where(W("Id", ids, DbWhereType.In)).Execute() > 0)
                    return DataStatus.Success;
                else
                    return DataStatus.Failed;
            }
            catch (Exception)
            {
                return DataStatus.Failed;
            }
        }

        public static FilterData<DataJoin<Product, StatisticData>> GetPageByParametersEx(DataSource ds, string categories, FilterParameters parameters, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = new DbWhereQueue();
            //if (parameters.Count > 0)
            //{
            //    foreach (long item in parameters.Keys)
            //        where &= (W<ProductAttributeMapping>("AttributeId", item) & P<ProductAttributeMapping>("Value", parameters[item]));
            //}

            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SupplierType"));
            orders.Add(D("SortNum"));
            orders.Add(D("SaleTime"));
            orders.Add(D("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    where &= W("Type", SaleType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    where &= W("Type", SaleType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    where &= W("Type", ViewType);
                    orders.Insert(0, D("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    where &= W("Type", ViewType);
                    orders.Insert(0, A("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D("Price"));
                    break;
            }

            IList<DataJoin<Product, StatisticData>> list = Db<Product>.Query(ds)
                .Select(S<Product>("*"), S<StatisticData>("*"), S_AS<Supplier>("SupplierType", "Level"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(where).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .Where(W<Product>("CategoryId", Array.ConvertAll(categories.Split(ImageSplitChar), new Converter<string, int>((x) => int.Parse(x))), DbWhereType.In) & GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>() & W<Product>("ProductType", productType))
                .OrderBy(orders.ToArray())
                .ToList<DataJoin<Product, StatisticData>>(size, parameters.Page, out count);

            return new FilterData<DataJoin<Product, StatisticData>>(parameters, new SplitPageData<DataJoin<Product, StatisticData>>(parameters.Page, size, list, count, show));
        }

        public static SplitPageData<Product> GetPage(DataSource ds, int categoryId, string q, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list;
            if (categoryId > 0)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("CategoryId", categoryId) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == 0)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -1)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.BeforeSale) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -2)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.BeforeSaved) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -3)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("Recommend", true) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -4)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("CategoryBest", 0, DbWhereType.GreaterThan) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -5)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("DiscountState", DiscountState.Approval) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else if (categoryId == -6)
                list = Db<Product>.Query(ds)
                .Select("Id", "Title", "Image", "CategoryId", "DiscountState", "State", "Recommend", "CategoryBest", "SortNum")
                .Where(W("State", ProductState.Deleted, DbWhereType.NotEqual) & W("DiscountState", DiscountState.Activated) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("Id"))
                .ToList<Product>(size, index, out count);
            else
            {
                if (!string.IsNullOrEmpty(q))
                {

                }
                throw new Exception();
            }
            return new SplitPageData<Product>(index, size, list, count, show);
        }

        public DataStatus UpdateState(DataSource ds, long supplierId, ProductState state)
        {
            return Update(ds, ColumnMode.Include, Cs("State"), WN("State", state, "Value") & P("SupplierId", supplierId) & (P("Id", Id) | P("ParentId", Id)));
        }

        public DataStatus UpdateInventory(DataSource ds, long supplierId)
        {
            return Update(ds, ColumnMode.Include, Cs(MODC("Inventory", Inventory)), (P("State", ProductState.BeforeSaved) | P("State", ProductState.Sale)) & P("SupplierId", supplierId) & P("Id", Id));
        }
        public static DataStatus UpdateInventoryById(DataSource ds, long id, int inventory)
        {
            return (new Product()).Update(ds, ColumnMode.Include, Cs(MODC("Inventory", -inventory)), WN("Inventory", inventory, "OldInventory", ">=") & (WN("State", ProductState.Sale, "State1") | WN("State", ProductState.BeforeSaved, "State2")) & P("Id", id));
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (ProductCategory.GetCountByParent(ds, CategoryId) > 0)
                return DataStatus.Exist;
            if (string.IsNullOrEmpty(Title))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "SupplierId", "ParentId");
            if (Include(columns, mode, "CategoryId") && ProductCategory.GetById(ds, CategoryId) == null)
                return DataStatus.Failed;
            if (Include(columns, mode, "Title") && string.IsNullOrEmpty(Title))
                return DataStatus.Failed;
            if (Include(columns, mode, "Content") && string.IsNullOrEmpty(Content))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static long GetCountByState(DataSource ds, bool sale, long userId, int productType = 1)
        {
            if (sale)
            {
                return Db<Product>.Query(ds)
                    .Select()
                    .Where((W("State", ProductState.Sale) | W("State", ProductState.BeforeSaved)) & W("SupplierId", userId) & W("ParentId", 0) & W("ProductType", productType))
                    .Count();
            }
            return Db<Product>.Query(ds)
                .Select()
                .Where((W("State", ProductState.Saved) | W("State", ProductState.BeforeSale)) & W("SupplierId", userId) & W("ParentId", 0) & W("ProductType", productType))
                .Count();
        }

        public static SplitPageData<Product> GetPageByWholesale(DataSource ds, int categoryId, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list;
            if (categoryId > 0)
                list = Db<Product>.Query(ds)
                    .Select()
                    .Where(GetParentWhereQueue() & GetStateWhereQueue() & W("CategoryId", categoryId) & W("Wholesale", true) & W("ProductType", productType))
                    .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                    .ToList<Product>(size, index, out count);
            else
                list = Db<Product>.Query(ds)
                    .Select()
                    .Where(GetParentWhereQueue() & GetStateWhereQueue() & W("Wholesale", true))
                    .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                    .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }

        public static SplitPageData<dynamic> ApiGetPageByCategory(DataSource ds, int categoryId, int categorylevel, FilterParameters2 parameters, int show = 8, int productType = 1)
        {
            long count;
            DbWhereQueue where = null;
            //关键词
            string[] qs;
            string q = parameters.KeyWord;

            if (string.IsNullOrEmpty(q))
            {
                qs = new string[] { string.Empty };
                where = W<Product>("Title", "", DbWhereType.Like);
            }
            else
            {
                qs = q.Split(' ');
                foreach (string s in qs)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (where == null)
                            where = W<Product>("Title", s, DbWhereType.Like);
                        else
                            where |= W<Product>("Title", s, DbWhereType.Like);
                    }
                }
            }
            where &= GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();
            //分类
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W<Product>("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            }
            //供应类型
            if (parameters.SupplierType != -1)
            {
                where &= W<Product>("SupplierType", parameters.SupplierType);
            }
            if (parameters.Brand > 0)
                where &= W<Product>("BrandId", parameters.Brand);
            //}

            //产品属性
            if (!string.IsNullOrEmpty(parameters.Attribute))
            {
                if (parameters.Attribute.IndexOf('@') != -1)
                {
                    string[] Attributes = parameters.Attribute.Split('@');
                    foreach (string Attr_Item in Attributes)
                    {
                        if (!string.IsNullOrEmpty(Attr_Item))
                        {
                            if (Attr_Item.IndexOf('_') != -1)
                            {
                                string[] Attr_Value = Attr_Item.Split('_');
                                if (!string.IsNullOrEmpty(Attr_Value[0]) && !string.IsNullOrEmpty(Attr_Value[1]))
                                {
                                    where &= (W<Product>("Id").InSelect<ProductAttributeMapping>(S("ProductId")).Where(W("AttributeId", long.Parse(Attr_Value[0].ToString())) & W("Value", Attr_Value[1].ToString())).Result());
                                }
                            }
                        }
                    }
                }
            }
            //店铺
            if (parameters.StoreId > 0)
            {
                where &= W<Product>("SupplierId", parameters.StoreId);
                //店铺分类
                if (parameters.StoreCategoryId > 0)
                {
                    if (Db<StoreCategory>.Query(ds).Select(S("ParentId")).Where(W("Id", parameters.StoreCategoryId)).First<StoreCategory>().ParentId > 0)
                    {
                        where &= W<Product>("StoreCategoryId", parameters.StoreCategoryId);
                    }
                    else
                    {
                        where &= (W<Product>("StoreCategoryId", parameters.StoreCategoryId) | W<Product>("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", parameters.StoreCategoryId)).Result());
                    }

                }
            }
            //价格
            if (!string.IsNullOrEmpty(parameters.Price))
            {
                if (parameters.Price.IndexOf('-') != -1)
                {
                    Money left = 0, right = 0;
                    string[] price = parameters.Price.Split('-');
                    if (price.Length > 1)
                    {
                        Money.TryParse(price[0], out left);
                        Money.TryParse(price[1], out right);
                        if (left != 0 && right != 0)
                        {
                            where &= (W<Product>("Price", left, DbWhereType.GreaterThanOrEqual) & W<Product>("Price", right, DbWhereType.LessThanOrEqual));
                        }
                        else
                        {
                            if (left != 0)
                                where &= W<Product>("Price", left, DbWhereType.GreaterThanOrEqual);
                            else
                                where &= W<Product>("Price", right, DbWhereType.LessThanOrEqual);
                        }
                    }
                }
            }
            //类型
            if (productType != 0)
                where &= W<Product>("ProductType", productType);

            //区域
            where &= (W<ProductSalesArea>("Province", parameters.Province) | W<ProductSalesArea>("Province", 0));
            where &= (W<ProductSalesArea>("City", parameters.City) | W<ProductSalesArea>("City", 0));
            where &= (W<ProductSalesArea>("County", parameters.County) | W<ProductSalesArea>("County", 0));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere &= W<ProductAreaMapping>("Province", parameters.Province);
            areawhere &= W<ProductAreaMapping>("City", parameters.City);
            areawhere &= W<ProductAreaMapping>("County", parameters.County);


            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SortNum"));
            orders.Add(D<Product>("SaleTime"));
            orders.Add(D<Product>("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A<Product>("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D<Product>("Price"));
                    break;
            }

            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("ProductType"), S<Product>("SupplierType"), S<Product>("CountyPrice"), S<Product>("DotPrice"), S<StatisticData>("Count"), S<StatisticData>("Type"), S<ProductAreaMapping>("CostPrice"), S<ProductAreaMapping>("CountyPrice"), S<ProductAreaMapping>("DotPrice"), S<ProductAreaMapping>("Price"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
                .LeftJoin(O<Product>("Id"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                .InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList(parameters.Size, parameters.Page, out count);
            if (list != null && list.Count > 0)
                StatisticTag.Add(ds, qs);
            return new SplitPageData<dynamic>(parameters.Page, parameters.Size, list, count, show);
        }
        [Obsolete]
        public static SplitPageData<dynamic> ApiGetPageByStoreCategory(DataSource ds, int StorecategoryId, int categorylevel, string keyword, FilterParameters parameters, int size, int show = 8, int productType = 1)
        {
            long count;
            DbWhereQueue where = null;
            string[] qs;
            string q = keyword;

            if (string.IsNullOrEmpty(q))
                qs = new string[] { string.Empty };
            else
                qs = q.Split(' ');
            foreach (string s in qs)
            {
                if (where == null)
                    where = W<Product>("Title", s, DbWhereType.Like);
                else
                    where |= W<Product>("Title", s, DbWhereType.Like);
            }
            where &= GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();
            if (StorecategoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W<Product>("StoreCategoryId", StorecategoryId);
                else if (categorylevel == 2)
                    where &= (W<Product>("StoreCategoryId", StorecategoryId) | W<Product>("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", StorecategoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W<Product>("StoreCategoryId", StorecategoryId) | W<Product>("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", StorecategoryId)).Result() | W<Product>("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", StorecategoryId)).Result()).Result());
            }

            DbWhereQueue subwhere = new DbWhereQueue();
            List<DbOrderBy> orders = new List<DbOrderBy>(2);
            orders.Add(D<Product>("SortNum"));
            orders.Add(D<Product>("SaleTime"));
            orders.Add(D<Product>("Id"));
            ProductOrderBy ob = (ProductOrderBy)parameters.OrderBy;
            switch (ob)
            {
                case ProductOrderBy.SaleDesc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.SaleAsc:
                    subwhere &= W<StatisticData>("Type", SaleType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewDesc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, D<StatisticData>("Count"));
                    break;
                case ProductOrderBy.ViewAsc:
                    subwhere &= W<StatisticData>("Type", ViewType);
                    orders.Insert(0, A<StatisticData>("Count"));
                    break;
                case ProductOrderBy.PriceAsc:
                    orders.Insert(0, A<Product>("Price"));
                    break;
                case ProductOrderBy.PriceDesc:
                    orders.Insert(0, D<Product>("Price"));
                    break;
            }
            where &= W<Product>("ProductType", productType);
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("ProductType"), S<Product>("SupplierType"), S<StatisticData>("Count"), S<StatisticData>("Type"))
                .LeftJoin(O<Product>("Id"), O<StatisticData>("TargetId")).Select().Where(subwhere).Result()
                .LeftJoin(O<Product>("SupplierId"), O<Distributor>("UserId"))
                .Where(where)
                .OrderBy(orders.ToArray())
                .ToList(size, parameters.Page, out count);
            if (list != null && list.Count > 0)
                StatisticTag.Add(ds, qs);
            return new SplitPageData<dynamic>(parameters.Page, size, list, count, show);
        }

        [Obsolete]
        public static SplitPageData<dynamic> GetPageByApi(DataSource ds, int categoryId, int categorylevel, long index, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = GetParentWhereQueue() & GetStateWhereQueue();
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            }
            else if (categoryId == -1)
                where &= W("DiscountState", DiscountState.Activated);
            else if (categoryId == -2)
                where &= (W("SupplierId", 812) | W("SupplierId", 772) | W("SupplierId", 215));
            else if (categoryId == -3)
                where &= W("Recommend", true);
            where &= W("ProductType", productType);
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"))
                .Where(where)
                .OrderBy(D<Product>("SortNum"), D<Product>("SaleTime"), D<Product>("Id"))
                .ToList(size, index, out count);

            return new SplitPageData<dynamic>(index, size, list, count, show);
        }
        [Obsolete]
        public static SplitPageData<dynamic> GetBrandPageByApi(DataSource ds, int categoryId, int categorylevel, int isbrand, long index, int size, int show = 8, int productType = 1)
        {
            long count;

            DbWhereQueue where = GetParentWhereQueue() & GetStateWhereQueue();
            if (isbrand == 0)
                where &= W("BrandId", 0, DbWhereType.Equal);
            else
                where &= W("BrandId", 0, DbWhereType.NotEqual);
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            }
            else if (categoryId == -1)
                where &= W("DiscountState", DiscountState.Activated);
            else if (categoryId == -2)
                where &= (W("SupplierId", 812) | W("SupplierId", 772) | W("SupplierId", 215));
            else if (categoryId == -3)
                where &= W("Recommend", true);
            where &= W<Product>("ProductType", productType);
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S("Id"), S("SortNum"), S("SaleTime"), S("Title"), S("Image"), S("DiscountState"), S("DiscountBeginTime"), S("DiscountEndTime"), S("DiscountPrice"), S("Price"), S("Wholesale"), S("WholesalePrice"), S("WholesaleCount"), S("WholesaleDiscount"))
                .Where(where)
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList(size, index, out count);

            return new SplitPageData<dynamic>(index, size, list, count, show);
        }
        [Obsolete]
        public static SplitPageData<dynamic> GetNewBrandPageByApi(DataSource ds, int categoryId, int categorylevel, int isbrand, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            DbWhereQueue where = GetParentWhereQueue<Product>() & GetStateWhereQueue<Product>();
            if (isbrand == 0)
                where &= W<Product>("BrandId", 0, DbWhereType.Equal);
            else
                where &= W<Product>("BrandId", 0, DbWhereType.NotEqual);
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W<Product>("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W<Product>("CategoryId", categoryId) | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W<Product>("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W<Product>("ParentId", categoryId)).Result()).Result());
            }
            else if (categoryId == -1)
                where &= W<Product>("DiscountState", DiscountState.Activated);
            else if (categoryId == -2)
                where &= (W<Product>("SupplierId", 812) | W<Product>("SupplierId", 772) | W<Product>("SupplierId", 215));
            else if (categoryId == -3)
                where &= W<Product>("Recommend", true);
            where &= W<Product>("ProductType", productType);
            IList<dynamic> list = Db<Product>.Query(ds)
               .Select(S<Product>("Id"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("SupplierType"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S_AS<Supplier>("SupplierType", "Level"))
               .LeftJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
               .Where(where)
               .OrderBy(D<Product>("SortNum"), D<Product>("SaleTime"), D<Product>("Id"))
               .ToList(size, index, out count);
            return new SplitPageData<dynamic>(index, size, list, count, show);
        }
        [Obsolete]
        public static IList<dynamic> GetPageByParentId(DataSource ds, long parentid)
        {
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S("*"))
                .Where(GetStateWhereQueue() & (W("ParentId", parentid) | W("Id", parentid)))
                .OrderBy(D("SaleTime"), D("Id"))
                .ToList();
            return list;
        }

        public static long[] GetAllIdsByParentId(DataSource ds, long parentId)
        {
            return Db<Product>.Query(ds).Select(S("Id")).Where(W("ParentId", parentId) | W("Id", parentId)).ToArray<long>();
        }


        public static DataStatus ModfiyByParentId(DataSource ds, Product product)
        {
            long parentId = product.ParentId > 0 ? product.ParentId : product.Id;
            if (Db<Product>.Query(ds).Update()
                .Set("Image", product.Image)
                .Set("State", product.State)
                .Set("Title", product.Title)
                .Set("Content", product.Content)
                .Set("Keywords", product.Keywords)
                .Set("Description", product.Description)
                .Set("BarCode", product.BarCode)
                .Set("Unit", product.Unit)
                .Set("Inventory", product.Inventory)
                .Set("InventoryAlert", product.InventoryAlert)
                .Set("Province", product.Province)
                .Set("City", product.City)
                .Set("County", product.County)
                .Set("FreightType", product.FreightType)
                .Set("FreightMoney", product.FreightMoney)
                .Set("FreightTemplate", product.FreightTemplate)
                .Set("HasReceipt", product.HasReceipt)
                .Set("StoreCategoryId", product.StoreCategoryId)
                .Set("Settlement", product.Settlement)
                .Set("RoyaltyRate", product.RoyaltyRate)
                .Set("ProductSupplier", product.ProductSupplier)
                .Set("Weight", product.Weight)
                .Set("Volume", product.Volume)
                .Where((W("ParentId", parentId) | W("Id", parentId))&W("State",ProductState.Deleted,DbWhereType.NotEqual))
                .Execute() > 0
                )
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

        public static IList<dynamic> GetPageByParentId2(DataSource ds, long parentid, int province, int city, int county)
        {
            DbWhereQueue where = GetStateWhereQueue<Product>() & (W<Product>("ParentId", parentid) | W<Product>("Id", parentid));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere = GetAreaMappingWhereQueue(province, city, county);
            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>(), S<ProductAreaMapping>("CostPrice"), S<ProductAreaMapping>("CountyPrice"), S<ProductAreaMapping>("DotPrice"), S<ProductAreaMapping>("Price"))
                .LeftJoin(O<Product>("Id"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                .Where(where)
                .OrderBy(D<Product>("SaleTime"), D<Product>("Id"))
                .ToList();
            return list;
        }
        public static IList<dynamic> GetPageByParentId2NoState(DataSource ds, long parentid, int province, int city, int county)
        {
            DbWhereQueue where = (W<Product>("ParentId", parentid) | W<Product>("Id", parentid));
            DbWhereQueue areawhere = new DbWhereQueue();
            areawhere &= W<ProductAreaMapping>("Province", province);
            areawhere &= W<ProductAreaMapping>("City", city);
            areawhere &= W<ProductAreaMapping>("County", county);

            IList<dynamic> list = Db<Product>.Query(ds)
                .Select(S<Product>(), S<ProductAreaMapping>("CostPrice"), S<ProductAreaMapping>("CountyPrice"), S<ProductAreaMapping>("DotPrice"), S<ProductAreaMapping>("Price"), S<ProductAreaMapping>("Saled"))
                .LeftJoin(O<Product>("Id"), O<ProductAreaMapping>("ProductId")).Select().Where(areawhere).Result()
                .Where(where)
                .OrderBy(D<Product>("SaleTime"), D<Product>("Id"))
                .ToList();
            return list;
        }
        [Obsolete]
        public static dynamic GetProductAndSupplierById(DataSource ds, int id)
        {
            dynamic product = Db<Product>.Query(ds)
               .Select(S<Product>(), S<Supplier>("Level"))
               .InnerJoin(O<Product>("SupplierId"), O<Supplier>("UserId"))
               .Where(W<Product>("Id", id))
               .First();
            if (product == null)
            {
                product = Db<Product>.Query(ds)
               .Select(S<Product>(), S<Distributor>("Level"))
               .InnerJoin(O<Product>("SupplierId"), O<Distributor>("UserId"))
               .Where(W<Product>("Id", id))
               .First();
            }
            return product;
        }


        /// <summary>
        /// 获取厂家或品牌团购
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="count"></param>
        /// <param name="isBrade">是否是品牌</param>
        /// <returns></returns>
        public static IList<Product> GetManufactorOrBrandGroupBy(DataSource ds, int count, int week, bool isBrade, int productType = 1)
        {
            DateTime now = DateTime.Now.AddDays(7 * week);
            DbWhereQueue where = W("DiscountBeginTime", now, DbWhereType.LessThanOrEqual) & W("DiscountEndTime", now, DbWhereType.GreaterThan) & GetStateWhereQueue() & W("DiscountState", DiscountState.Activated) & W("ParentId", 0);
            if (isBrade)
                where &= W("BrandId", 0, DbWhereType.NotEqual);
            else
                where &= W("BrandId", 0);
            where &= W("ProductType", productType);
            return Db<Product>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(A("DiscountEndTime"), D("Id"))
                .ToList<Product>(count);
        }

        public static DataStatus UpdateStoreRecommend(DataSource ds, long id, bool storeRecommend, long userId)
        {
            return Db<Product>.Query(ds).Update().Set("StoreRecommend", storeRecommend).Where(W("Id", id) & W("SupplierId", userId)).Execute() > 0 ? DataStatus.Success : DataStatus.Failed;
        }

        public static DataStatus UpdateStoreSortNum(DataSource ds, long id, int storeSortNum, long userId)
        {
            return Db<Product>.Query(ds).Update().Set("StoreSortNum", storeSortNum).Where(W("Id", id) & W("SupplierId", userId)).Execute() > 0 ? DataStatus.Success : DataStatus.Failed;
        }

        public static IList<Product> GetProductListByStoreCategoryId(DataSource ds, int storeCategoryId, int count, int productType = 1)
        {
            return Db<Product>.Query(ds)
                .Select()
                .Where(GetParentWhereQueue() & GetStateWhereQueue() & W("StoreCategoryId", storeCategoryId) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id")).ToList<Product>(count);
        }

        public static SplitPageData<Product> GetProductListByStoreCategoryId(DataSource ds, int storeCategoryId, long index, int size, int show = 8, int productType = 1)
        {
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(GetParentWhereQueue() & GetStateWhereQueue() & W("StoreCategoryId", storeCategoryId) & W("ProductType", productType))
                .OrderBy(D("SortNum"), D("SaleTime"), D("Id"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }

        public static long GetProductCountBySupplierId(DataSource ds, long supplierId)
        {
            return Db<Product>.Query(ds).Select().Where(GetParentWhereQueue() & GetStateWhereQueue() & W("SupplierId", supplierId)).Count();
        }

        /// <summary>
        /// 获取商品销量总数
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public long GetProductSalesCount(DataSource ds)
        {
            return Db<ProductOrder>.Query(ds).Select().
                InnerJoin(O<ProductOrder>("Id"), O<ProductOrderMapping>("OrderId")).
                Where(W<ProductOrder>("State", 6) & W<ProductOrderMapping>("IsService", 0) & W<ProductOrderMapping>("ProductId", this.Id)).Count();
        }

        /// <summary>
        /// 获取商品评论总数
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public long GetProductCommentCount(DataSource ds)
        {
            return Db<ProductOrder>.Query(ds).Select().
                InnerJoin(O<ProductOrder>("Id"), O<ProductOrderMapping>("OrderId")).
                Where(W<ProductOrder>("State", 6) & W<ProductOrderMapping>("Evaluation", 1) & W<ProductOrderMapping>("ProductId", this.Id)).Count();
        }

        public static string GetFreight(DataSource ds, string provice, string city, long productId)
        {
            Product product = Product.GetById(ds, productId);
            if (product.FreightType == FreightType.Fix)
                return product.GetFreightStringImpl(product.FreightMoney);

            FreightTemplate tmp = Modules.FreightTemplate.GetById(ds, product.FreightTemplate);
            if (tmp != null)
            {
                using (Cnaws.Area.Country country = Cnaws.Area.Country.GetCountry())
                {
                    FreightMapping map = FreightMapping.GetMapping(ds, tmp.Id, country.GetProvince(provice).Id, country.GetCity(city).Id);
                    if (map != null)
                    {
                        if (map.StepNumber > 0)
                            return string.Concat(product.GetFreightStringImpl(map.Money), "&nbsp;满", product.GetFreightStringImpl(map.StepNumber), "&nbsp;", product.GetFreightStringImpl(map.StepMoney));
                        return product.GetFreightStringImpl(map.Money);
                    }
                }
            }
            return product.GetFreightStringImpl(0);
        }

        #region 乡道馆产品
        #region 通过产品Id查询产品信息
        /// <summary>
        /// 通过产品Id查询产品信息
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static Product GetProductByProductId(DataSource ds, long productId)
        {
            return Db<Product>.Query(ds)
                .Select(S<Product>("*"))
                .Where(GetParentWhereQueue() & GetStateWhereQueue() & W<Product>("Id", productId)).First<Product>();
        }
        #endregion

        #region 通过乡道馆分类Id查询产品信息集合
        /// <summary>
        /// 通过乡道馆分类Id查询产品信息集合
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="XDGCategoryId"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public static SplitPageData<Product> GetProductListByXDGCategoryId(DataSource ds, int XDGCategoryId, int categorylevel, long index, int size, int show = 8)
        {
            DbWhereQueue where = GetParentWhereQueue() & GetStateWhereQueue() & W<Product>("ProductType", 2);
            if (XDGCategoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W("StoreCategoryId", XDGCategoryId);
                else if (categorylevel == 2)
                    where &= (W("StoreCategoryId", XDGCategoryId) | W("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", XDGCategoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W("StoreCategoryId", XDGCategoryId) | W("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", XDGCategoryId)).Result() | W("CategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", XDGCategoryId)).Result()).Result());
            }
            long count;
            IList<Product> list = Db<Product>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(D("SortNum"), D("SaleTime"), A("Id"))
                .ToList<Product>(size, index, out count);
            return new SplitPageData<Product>(index, size, list, count, show);
        }
        #endregion

        #region 通过乡道馆信息Id查询乡道馆首页推荐商品
        /// <summary>
        /// 通过乡道馆信息Id查询乡道馆首页推荐商品
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId">乡道馆Id</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IList<Product> GetProduct(DataSource ds, long userId, int pageSize)
        {
            IList<Product> list = Db<Product>.Query(ds).Select().
                Where(Product.GetParentWhereQueue() & Product.GetStateWhereQueue() & W<Product>("StoreRecommend", true) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2)).
                OrderBy(A("StoreSortNum")).
                ToList<Product>(pageSize);
            return list;
        }

        public static SplitPageData<dynamic> GetProductApi(DataSource ds, int page, long userId, int pageSize)
        {
            long count;
            IList<dynamic> list = Db<Product>.Query(ds).Select(S<Product>("Description"), S<Product>("Id"), S<Product>("StoreSortNum"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("ProductType")).
                Where(Product.GetParentWhereQueue<Product>() & Product.GetStateWhereQueue<Product>() & W<Product>("StoreRecommend", true) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2)).
                OrderBy(A<Product>("StoreSortNum")).
               ToList(pageSize, page, out count);
            return new SplitPageData<dynamic>(page, pageSize, list, count, 8);
        }
        public static SplitPageData<dynamic> GetProductApiByArea(DataSource ds, int province, int city, int county, int page, long userId, int pageSize)
        {
            long count;
            IList<dynamic> list = Db<Product>.Query(ds).Select(S<Product>("Description"), S<Product>("Id"), S<Product>("StoreSortNum"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("ProductType"))
                .InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId"))
                .Where(Product.GetParentWhereQueue<Product>() & Product.GetStateWhereQueue<Product>() & W<Product>("StoreRecommend", true) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2)
                & (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0))
                & (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0))
                & (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0)))
                .OrderBy(A<Product>("StoreSortNum")).
               ToList(pageSize, page, out count);
            return new SplitPageData<dynamic>(page, pageSize, list, count, 8);
        }
        public static IList<Product> GetProduct(DataSource ds, long userId, int pageSize, DateTime dateTime)
        {
            IList<Product> list = Db<Product>.Query(ds).Select().
                Where(Product.GetParentWhereQueue() & Product.GetStateWhereQueue() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2) & W("DiscountBeginTime", dateTime, DbWhereType.LessThanOrEqual) & W("DiscountEndTime", dateTime, DbWhereType.GreaterThan)).
                ToList<Product>(pageSize);
            return list;
        }
        public static SplitPageData<dynamic> GetProductApi(DataSource ds, long userId, int page, int pageSize, DateTime dateTime)
        {
            long count;
            IList<dynamic> list = Db<Product>.Query(ds).Select(S<Product>("Description"), S<Product>("Id"), S<Product>("StoreSortNum"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("ProductType")).
                Where(Product.GetParentWhereQueue<Product>() & Product.GetStateWhereQueue<Product>() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2) & W("DiscountBeginTime", dateTime, DbWhereType.LessThanOrEqual) & W("DiscountEndTime", dateTime, DbWhereType.GreaterThan)).
                OrderBy(A<Product>("StoreSortNum")).
                ToList(pageSize, page, out count);

            return new SplitPageData<dynamic>(page, pageSize, list, count, 8);
        }
        public static SplitPageData<dynamic> GetProductApiByArea(DataSource ds, int province, int city, int county, long userId, int page, int pageSize, DateTime dateTime)
        {
            long count;
            IList<dynamic> list = Db<Product>.Query(ds).Select(S<Product>("Description"), S<Product>("Id"), S<Product>("StoreSortNum"), S<Product>("SortNum"), S<Product>("SaleTime"), S<Product>("Title"), S<Product>("Image"), S<Product>("DiscountState"), S<Product>("DiscountBeginTime"), S<Product>("DiscountEndTime"), S<Product>("Wholesale"), S<Product>("WholesalePrice"), S<Product>("WholesaleCount"), S<Product>("WholesaleDiscount"), S<Product>("DiscountPrice"), S<Product>("Price"), S<Product>("MarketPrice"), S<Product>("ProductType")).
                InnerJoin(O<Product>("Id"), O<ProductSalesArea>("ProductId")).
                Where(Product.GetParentWhereQueue<Product>() & Product.GetStateWhereQueue<Product>() & W<Product>("DiscountState", DiscountState.Activated) & W<Product>("SupplierId", userId) & W<Product>("ProductType", 2) & W("DiscountBeginTime", dateTime, DbWhereType.LessThanOrEqual) & W("DiscountEndTime", dateTime, DbWhereType.GreaterThan)
                & (W<ProductSalesArea>("Province", province) | W<ProductSalesArea>("Province", 0))
                & (W<ProductSalesArea>("City", city) | W<ProductSalesArea>("City", 0))
                & (W<ProductSalesArea>("County", county) | W<ProductSalesArea>("County", 0))).
                OrderBy(A<Product>("StoreSortNum")).
                ToList(pageSize, page, out count);
            return new SplitPageData<dynamic>(page, pageSize, list, count, 8);
        }

        #endregion

        #region 通过分类Id查询分类下面的商品总数
        /// <summary>
        /// 通过分类Id查询分类下面的商品总数
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="storeCategoryId">分类Id</param>
        /// <returns></returns>
        public static long GetProductCount(DataSource ds, int storeCategoryId)
        {
            return Db<Product>.Query(ds)
                    .Select()
                    .Where(Product.GetParentWhereQueue() & Product.GetStateWhereQueue() & W<Product>("StoreCategoryId", storeCategoryId) & W<Product>("ProductType", 2))
                    .Count();
        }
        #endregion 
        #endregion


    }
}
