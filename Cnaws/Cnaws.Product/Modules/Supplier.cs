using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;
using Cnaws.Templates;
using System.Collections.Generic;
using Cnaws.Area;

namespace Cnaws.Product.Modules
{
    public enum SupplierState
    { 
        /// <summary>
        /// 未支付
        /// </summary>
        NotPaid = 0,
        /// <summary>
        /// 未完善
        /// </summary>
        Incomplete = 1,
        /// <summary>
        /// 未审核
        /// </summary>
        NotApproved = 2,
        /// <summary>
        /// 审核失败
        /// </summary>
        ApprovedFailure = 3,
        /// <summary>
        /// 已过期
        /// </summary>
        Expired = 4,
        /// <summary>
        /// 已审核
        /// </summary>
        Approved = 5,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = -1

    }

    public enum SettlementType
    {
        /// <summary>
        /// 固定价格
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// 提成
        /// </summary>
        Royalty = 1
    }

    /// <summary>
    /// 供应商
    /// </summary>
    [Serializable]
    public sealed class Supplier : NoIdentityModule
    {
        public const char ImageSplitChar = '|';
        public const char CategorySplitChar = ',';

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataColumn(true)]
        public long UserId = 0L;
        /// <summary>
        /// 隶属加盟商
        /// </summary>
        public long Subjection = 0L;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level = 0;
        /// <summary>
        /// 供应商类型1为本地供应商,0为全国供应商
        /// </summary>
        public int SupplierType = 0;
        /// <summary>
        /// 公司名称
        /// </summary>
        [DataColumn(32)]
        public string Company = null;
        /// <summary>
        /// 证件图片，用“|”分隔
        /// </summary>
        public string Images = null;
        /// <summary>
        /// 签约人
        /// </summary>
        [DataColumn(16)]
        public string Signatories = null;
        /// <summary>
        /// 签约人联系电话
        /// </summary>
        [DataColumn(16)]
        public string SignatoriesPhone = null;
        /// <summary>
        /// 负责人
        /// </summary>
        [DataColumn(16)]
        public string Contact = null;
        /// <summary>
        /// 负责人联系电话
        /// </summary>
        [DataColumn(16)]
        public string ContactPhone = null;
        public int Province = 0;
        public int City = 0;
        public int County = 0;
        /// <summary>
        /// 公司地址
        /// </summary>
        [DataColumn(128)]
        public string Address = null;
        /// <summary>
        /// 行业，用“,”分隔
        /// </summary>
        public string Categories = null;
        /// <summary>
        /// 是否开启邮费活动
        /// </summary>
        public bool IsActivityFree = false;
        /// <summary>
        /// 活动条件（满足条件则开启邮费活动）
        /// </summary>
        public Money ActivityCondition = 0;
        /// <summary>
        /// 满足条件后的活动邮费
        /// </summary>
        public Money ActivityFree = 0;
        /// <summary>
        /// 主营产品
        /// </summary>
        [DataColumn(128)]
        public string Products = null;
        /// <summary>
        /// 保证金
        /// </summary>
        public Money Money = 0;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 签约年限
        /// </summary>
        public int Year = 0;
        /// <summary>
        /// 状态
        /// </summary>
        public SupplierState State = SupplierState.NotPaid;
        /// <summary>
        /// 结算方式
        /// </summary>
        public SettlementType Settlement = SettlementType.Fixed;
        /// <summary>
        /// 提成率
        /// </summary>
        public int RoyaltyRate = 0;
        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message = null;
        /// <summary>
        /// QQ
        /// </summary>
        [DataColumn(256)]
        public string QQ = null;
        /// <summary>
        /// 店铺最小订单金额（仅使用进货宝,满足条件才能下单）
        /// </summary>
        public Money MinOrderPrice = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "State");
            DropIndex(ds, "Subjection");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "State", "State");
            CreateIndex(ds, "Subjection", "Subjection");
        }

        public double GetRoyalty(ProductOrder order)
        {
            if (Settlement == SettlementType.Royalty)
            {
                if (RoyaltyRate <= 0)
                    return (double)(order.TotalMoney - order.FreightMoney) * 0.15;
                return (double)(order.TotalMoney - order.FreightMoney) * (RoyaltyRate / 100.0);
            }
            return 0.0;
        }

        public string GetStateString()
        {
            switch (State)
            {
                case SupplierState.Approved: return "已审核";
                case SupplierState.ApprovedFailure: return "审核失败";
                case SupplierState.Expired: return "已过期";
                case SupplierState.Incomplete: return "未完善";
                case SupplierState.NotApproved: return "未审核";
                case SupplierState.NotPaid: return "未支付";
            }
            return string.Empty;
        }
        public string GetSettlementString()
        {
            switch (Settlement)
            {
                case SettlementType.Fixed: return "自营";
                case SettlementType.Royalty: return "代卖";
            }
            return string.Empty;
        }

        public string[] GetImages()
        {
            if (Images != null)
                return Images.Split(ImageSplitChar);
            return new string[] { };
        }
        public IList<ProductCategory> GetCategories(DataSource ds)
        {
            if (!string.IsNullOrEmpty(Categories))
            {
                int[] array = Array.ConvertAll(Categories.Trim(',').Split(CategorySplitChar), new Converter<string, int>((x) => int.Parse(x)));
                return Db<ProductCategory>.Query(ds)
                    .Select()
                    .Where(W("Id", array, DbWhereType.In))
                    .ToList<ProductCategory>();
            }
            return new List<ProductCategory>();
        }
        public int GetDays()
        {
            int days = (int)Math.Floor((CreationDate.AddYears(Year) - DateTime.Now).TotalDays);
            if (days > 0)
                return days;
            return 0;
        }

        public dynamic GetSaleArea(DataSource ds)
        {
            Distributor dis = Db<Distributor>.Query(ds).Select().Where(W("UserId", Subjection)).First<Distributor>();
            return new { Province = dis.Province, City = dis.City, County = dis.County };
        }


        public static bool IsSupplier(DataSource ds, long userId)
        {
            return Db<Supplier>.Query(ds)
                .Select()
                .Where(W("UserId", userId))
                .Count() > 0;
        }
        public static Supplier GetById(DataSource ds, long userId)
        {
            return Db<Supplier>.Query(ds)
                .Select()
                .Where(W("UserId", userId)&W("State",-1,DbWhereType.NotEqual))
                .First<Supplier>();
        }
        public static IList<DataJoin<Supplier,StoreInfo>> GetAndStorInfoByIds(DataSource ds, long[] userIds)
        {
            return Db<Supplier>.Query(ds)
                .Select(S<Supplier>("UserId"), S<Supplier>("Company"),
                S<Supplier>("IsActivityFree"),
                S<Supplier>("ActivityCondition"),
                S<Supplier>("ActivityFree"),
                S<Supplier>("MinOrderPrice"),
                S<StoreInfo>("StoreName"), S<StoreInfo>("StoreLogo"), S<StoreInfo>("StoreSlogan"), S<StoreInfo>("StoreNotice"), S<StoreInfo>("StoreExplain"))
                .LeftJoin(O<Supplier>("UserId"), O<StoreInfo>("UserId"))
                .Where(W<Supplier>("UserId", userIds, DbWhereType.In) & W<Supplier>("State", -1, DbWhereType.NotEqual))
                .ToList<DataJoin<Supplier, StoreInfo>>();
        }
        public static IList<dynamic> GetDynamicAndStorInfoByIds(DataSource ds, long[] userIds)
        {
            return Db<Supplier>.Query(ds)
                .Select(S<Supplier>("UserId"), S<Supplier>("Company"),
                S<Supplier>("IsActivityFree"),
                S<Supplier>("ActivityCondition"),
                S<Supplier>("ActivityFree"),
                S<Supplier>("MinOrderPrice"),
                S<StoreInfo>("StoreName"), S<StoreInfo>("StoreLogo"), S<StoreInfo>("StoreSlogan"), S<StoreInfo>("StoreNotice"), S<StoreInfo>("StoreExplain"))
                .LeftJoin(O<Supplier>("UserId"),O<StoreInfo>("UserId"))
                .Where(W<Supplier>("UserId", userIds,DbWhereType.In) & W<Supplier>("State", -1, DbWhereType.NotEqual))
                .ToList();
        }
        public DataStatus UpdateWithState(DataSource ds)
        {
            if (Update(ds, ColumnMode.Include, Cs("MinOrderPrice", "IsActivityFree", "ActivityCondition", "ActivityFree", "Company","Province","City","County" ,"Images", "Signatories", "SignatoriesPhone", "Contact", "ContactPhone", "Address", "Categories", "Products","QQ"), WN("State", SupplierState.Approved, "State", "<>") & P("UserId", UserId)) == DataStatus.Success)
                return DataStatus.Success;
            return Update(ds, ColumnMode.Include, "MinOrderPrice", "IsActivityFree", "ActivityCondition", "ActivityFree", "Company", "Province", "City", "County", "Images", "Signatories", "SignatoriesPhone", "Contact", "ContactPhone", "Address", "Categories", "Products", "QQ");
            //return Update(ds, ColumnMode.Include, "IsActivityFree", "ActivityCondition", "ActivityFree", "Contact", "City", "County", "Images", "ContactPhone", "Address", "Products", "Categories");
        }
        public DataStatus Approved(DataSource ds)
        {
            return Update(ds, ColumnMode.Include, Cs("CreationDate", "Year", "Settlement", "RoyaltyRate", "State", "Message"), /*WN("State", SupplierState.NotApproved, "OldState") &*/ P("UserId", UserId));
        }

        public static SplitPageData<Supplier> GetPage(DataSource ds, int state, long index, int size, int show = 8)
        {
            long count;
            IList<Supplier> list;
            if (state == -10)
                list = Db<Supplier>.Query(ds)
                .Select()
                .OrderBy(D("CreationDate"))
                .ToList<Supplier>(size, index, out count);
            else if (state <= 0)
                list = Db<Supplier>.Query(ds)
                .Select()
                .Where(W("State", -state))
                .OrderBy(D("CreationDate"))
                .ToList<Supplier>(size, index, out count);
            else if (state > 0)
                list = Db<Supplier>.Query(ds)
                .Select()
                .Where(W("Categories", string.Concat(',', state, ','), DbWhereType.Like) | W("Categories", string.Concat(',', state), DbWhereType.LikeEnd) | W("Categories", string.Concat(state, ','), DbWhereType.LikeBegin) | W("Categories", state.ToString()))
                .OrderBy(D("CreationDate"))
                .ToList<Supplier>(size, index, out count);
            else
                throw new Exception();
            return new SplitPageData<Supplier>(index, size, list, count, show);
        }

        public static SplitPageData<Supplier> GetBySubjection(DataSource ds,long subjectionid,string state, long index, int size, int show = 8)
        {
            long count;
            DbWhereQueue where = new DbWhereQueue();
            where = W("Subjection", subjectionid);
            if (!string.IsNullOrEmpty(state))
                where &= W("State", state);
            IList<Supplier> List= Db<Supplier>.Query(ds).Select().Where(where).OrderBy(D("UserId")).ToList<Supplier>(size, index, out count);
            return new SplitPageData<Supplier>(index, size, List, count, show);


        }

    }
}
