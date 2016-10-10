using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;
using System.Collections.Generic;
using Cnaws.Area;

namespace Cnaws.Product.Modules
{
    public enum DistributorState
    {
        /// <summary>
        /// 未审核
        /// </summary>
        NotApproved = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        Approved = 1,
        /// <summary>
        /// 审核失败
        /// </summary>
        ApprovedFailure = 2,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = -1
    }

    /// <summary>
    /// 加盟商
    /// </summary>
    [Serializable]
    public sealed class Distributor : NoIdentityModule
    {
        public const char ImageSplitChar = '|';
        /// <summary>
        /// 收货人
        /// </summary>
        [DataColumn(32)]
        public string Consignee = null;
        /// <summary>
        /// 收货人手机号
        /// </summary>
        public long Mobile = 0L;
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataColumn(true)]
        public long UserId = 0L;
        public long ParentId = 0L;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level = 0;
        public DistributorState State;
        public Money Money = 0;
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
        public int PostId = 0;
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude = 0.0;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude = 0.0;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ParentId");
            DropIndex(ds, "State");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ParentId", "ParentId");
            CreateIndex(ds, "State", "State");
        }

        public string[] GetImages()
        {

            if (Images != null)
                return Images.Split(ImageSplitChar);
            return new string[] { };
        }
        public string GetLevelString()
        {
            switch (Level)
            {
                case 0: return "省级加盟商";
                case 1: return "县级加盟商";
                case 2: return "镇级旗舰店";
                case 3: return "镇级中心店";
                case 100:return "会员加盟店";
                default: return "村级加盟店";
            }
        }
        /// <summary>
        /// 实例方法，在前台可以直接调用，查询地址信息
        /// </summary>
        /// <returns></returns>
        public string GetAddressStr()
        {
            string ProvinceStr, CityStr, Countystr;
            using (Country c = Country.GetCountry())
            {
                if (Province != 0)
                    ProvinceStr = c.GetCity(Province).Name;
                else
                    ProvinceStr = "";
                if (City != 0)
                    CityStr = c.GetCity(City).Name;
                else
                    CityStr = "";
                if (County != 0)
                    Countystr = c.GetCity(County).Name;
                else
                    Countystr = "";
            }
            return ProvinceStr + " " + CityStr + " " + Countystr + " " + Address;

        }


        public double GetRoyalty(ProductOrder order)
        {
            double money = (double)(order.TotalMoney - order.FreightMoney);
            if (order.ShopId == UserId)
                return money * 0.05;
            switch (Level)
            {
                case 0: return money * 0.01;
                case 1: return money * 0.02;
                default: return 0.0;
            }
        }
        public double GetRoyaltyByOrderMapping(DataJoin<ProductOrder, ProductOrderMapping> order)
        {
            double money = (double)(order.B.TotalMoney);
            switch (Level)
            {
                case 0:
                    if (order.A.ShopId == UserId) return money * 0.09;
                    else return money * 0.01;
                case 1:
                    if (order.A.ShopId == UserId) return money * 0.08;
                    else return money * 0.02;
                case 2:
                    if (order.A.ShopId == UserId) return money * 0.05;
                    else return money * 0.01;
                default: return 0.0;
            }
        }
        public double GetRoyaltyByOrderMapping(long shopId)
        {
            switch (Level)
            {
                case 0:
                    if (shopId == UserId) return 0.01;
                    else return 0.01;
                case 1:
                    if (shopId == UserId) return 0.02;
                    else return 0.02;
                case 2:
                    if (shopId == UserId) return 0.05;
                    else return 0.01;
                case 3:
                    if (shopId == UserId) return 0.05;
                    else return 0.01;
                case 4:
                    if (shopId == UserId) return 0.05;
                    else return 0.0;
                default: return 0.0;
            }
        }
        public double GetRoyaltyByDistributorOrderMapping(long shopId)
        {
            switch (Level)
            {
                case 0:
                    return 0.01;
                case 1:
                    return 0.02;
                case 2:
                    if (shopId != UserId) return 0.01;
                    else return 0.0;
                case 3:
                    if (shopId != UserId) return 0.01;
                    else return 0.0;
                default: return 0.0;
            }
        }
        public string GetStateString()
        {
            switch (State)
            {
                case DistributorState.NotApproved: return "已提交审核";
                case DistributorState.Approved: return "审核通过";
                case DistributorState.Deleted:return "已删除";
                default: return "审核失败";
            }
        }

        public static bool IsDistributor(DataSource ds, long userId)
        {
            return Db<Distributor>.Query(ds)
                .Select()
                .Where(W("UserId", userId) & W("State", -1, DbWhereType.NotEqual))
                .Count() > 0;
        }
        public static Distributor GetById(DataSource ds, long userId)
        {
            return Db<Distributor>.Query(ds)
                .Select()
                .Where(W("UserId", userId) & W("State", -1, DbWhereType.NotEqual))
                .First<Distributor>();
        }
        public static Distributor GetByAreaAndLevel(DataSource ds, int province, int city, int county, int level)
        {
            return Db<Distributor>.Query(ds)
                .Select()
                .Where(W("Province", province)& W("City", city) & W("County", county)&W("Level",level) & W("State", -1, DbWhereType.NotEqual))
                .First<Distributor>();
        }

        public static long GetNumByParentId(DataSource ds, long parentId)
        {
            return Db<Distributor>.Query(ds).Select().Where(W("ParentId", parentId)).Count();
        }

        public DataStatus UpdateWithState(DataSource ds)
        {
            return Update(ds, ColumnMode.Include, "Company", "Images", "Signatories", "SignatoriesPhone", "Contact","Province","City","County", "ContactPhone", "Address", "PostId");
        }
        public DataStatus UpdateAddressWithState(DataSource ds)
        {
            return Update(ds, ColumnMode.Include, "Consignee", "Mobile", "Address");
        }
        public static DataStatus ModifyMoney(DataSource ds, long id, Money value)
        {
            if (value == 0)
                return DataStatus.Success;
            DataWhereQueue where = P("UserId", id);
            if (value < 0)
                where = WN("Money", -value, "OldMoney", ">=") & where;
            return (new Distributor() { UserId = id }).Update(ds, ColumnMode.Include, Cs(MODC("Money", value)), where);
        }

        public static SplitPageData<Distributor> GetPageByParent(DataSource ds, long parent, long index, int size, int show = 8)
        {
            long count;
            IList<Distributor> list = Db<Distributor>.Query(ds)
                .Select()
                .Where(W("ParentId", parent) & W("State", -1, DbWhereType.NotEqual))
                .OrderBy(D("CreationDate"))
                .ToList<Distributor>(size, index, out count);
            return new SplitPageData<Distributor>(index, size, list, count, show);
        }
        /// <summary>
        /// 根据父id获得当前子级的联系人
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Distributor GetContactByParent(DataSource ds, long parent)
        {
            return Db<Distributor>.Query(ds)
                 .Select()
                 .Where(W("ParentId", parent) & W("State", -1, DbWhereType.NotEqual))
                 .OrderBy(D("CreationDate"))
                 .First<Distributor>();

        }
        /// <summary>
        /// 根据当前登录用户的ID查询他的加盟商级别
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Distributor GetLevelByUserId(DataSource ds, long userId)
        {
            return Db<Distributor>.Query(ds)
                 .Select("Level")
                 .Where(W("UserId", userId) & W("State", -1, DbWhereType.NotEqual))
                 .OrderBy(D("CreationDate"))
                 .First<Distributor>();
        }

        public static DataStatus Approved(DataSource ds,long userid,DistributorState state)
        {
            return new Distributor() { UserId = userid, State = state }.Update(ds, ColumnMode.Include, new DataColumn[] { "State" }, P("UserId", userid));
        }



        /// <summary>
        /// 根据父id获得子id
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Distributor GetUserIdByParent(DataSource ds, long parent)
        {
            IList<Distributor> list = Db<Distributor>.Query(ds)
                .Select()
                .Where(W("ParentId", parent) & W("State", -1, DbWhereType.NotEqual))
                .OrderBy(D("CreationDate"))
                .ToList<Distributor>();
            return new Distributor();
        }
        //public static SplitPageData<Distributor> GetPage(DataSource ds, string state, long index, int size, int show = 11)
        //{
        //    long count;
        //    IList<Distributor> list;
        //    if ("_".Equals(state))
        //    {
        //        list = Db<Distributor>.Query(ds)
        //            .Select()
        //            .OrderBy(D("CreationDate"))
        //            .ToList<Distributor>(size, index, out count);
        //    }
        //    else
        //    {
        //        list = Db<Distributor>.Query(ds)
        //            .Select()
        //            .Where(W("State", Enum.Parse(TType<DistributorState>.Type, state)))
        //            .OrderBy(D("CreationDate"))
        //            .ToList<Distributor>(size, index, out count);
        //    }
        //    return new SplitPageData<Distributor>(index, size, list, count, show);
        //}

        public static dynamic GetMoneyByParent(DataSource ds, long parent, long index, int size, int show = 8)
        {
            Distributor dis = GetById(ds, parent);

            int level = dis.Level;

            long[] temp;
            List<long> all = new List<long>();
            all.Add(parent);
            Dictionary<int, long[]> dict = new Dictionary<int, long[]>();
            dict.Add(level, new long[] { parent });

            while (++level < 5)
            {
                if (dict.TryGetValue(level - 1, out temp) && temp != null && temp.Length > 0)
                {
                    temp = Db<Distributor>.Query(ds)
                        .Select(S("UserId"))
                        .Where(W("ParentId", temp, DbWhereType.In) & W("State", -1, DbWhereType.NotEqual))
                        .ToArray<long>();
                    if (temp != null && temp.Length > 0)
                    {
                        all.AddRange(temp);
                        dict.Add(level, temp);
                    }
                }
            }

            double value;
            double total = 0;
            double money = 0;
            DateTime now = DateTime.Now.AddDays(-7);
            IList<ProductOrder> orders = Db<ProductOrder>.Query(ds)
                .Select(S("ShopId"), S("TotalMoney"), S("FreightMoney"), S("ReceiptDate"))
                .Where(W("ShopId", all, DbWhereType.In) & (/*W("State", OrderState.Evaluation) | */W("State", OrderState.Finished)))
                .ToList<ProductOrder>();
            foreach (ProductOrder order in orders)
            {
                value = dis.GetRoyalty(order);
                total += value;
                if (order.ReceiptDate <= now)
                    money += value;
            }

            long count;
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(W("ShopId", all, DbWhereType.In) & (/*W("State", OrderState.Evaluation) | */W("State", OrderState.Finished)))
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            SplitPageData<ProductOrder> data = new SplitPageData<ProductOrder>(index, size, list, count, show);

            return new
            {
                Total = total,
                Money = money,
                Data = data
            };
        }

        public static DataStatus UpdateConsignee(DataSource ds,Distributor distributor)
        {
            if (Db<Distributor>.Query(ds).Update().Set("Consignee", distributor.Consignee).Set("Mobile", distributor.Mobile)
                .Set("Address", distributor.Address).Execute() > 0)
            {
                return DataStatus.Success;
            }
            else
                return DataStatus.Failed;
        }


        public static dynamic GetCash(DataSource ds, long parent)
        {
            Distributor dis = GetById(ds, parent);

            int level = dis.Level;

            long[] temp;
            List<long> all = new List<long>();
            all.Add(parent);
            Dictionary<int, long[]> dict = new Dictionary<int, long[]>();
            dict.Add(level, new long[] { parent });

            while (++level < 5)
            {
                if (dict.TryGetValue(level - 1, out temp) && temp != null && temp.Length > 0)
                {
                    temp = Db<Distributor>.Query(ds)
                        .Select(S("UserId"))
                        .Where(W("ParentId", temp, DbWhereType.In) & W("State", -1, DbWhereType.NotEqual))
                        .ToArray<long>();
                    if (temp != null && temp.Length > 0)
                    {
                        all.AddRange(temp);
                        dict.Add(level, temp);
                    }
                }
            }

            double value;
            double total = 0;
            double money = 0;
            DateTime now = DateTime.Now.AddDays(-7);
            IList<ProductOrder> orders = Db<ProductOrder>.Query(ds)
                .Select(S("ShopId"), S("TotalMoney"), S("FreightMoney"), S("ReceiptDate"))
                .Where(W("ShopId", all, DbWhereType.In) & (/*W("State", OrderState.Evaluation) | */W("State", OrderState.Finished)))
                .ToList<ProductOrder>();
            foreach (ProductOrder order in orders)
            {
                value = dis.GetRoyalty(order);
                total += value;
                if (order.ReceiptDate <= now)
                    money += value;
            }

            return new
            {
                Total = total,
                Money = money
            };
        }
    }
}
