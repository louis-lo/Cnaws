using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;
using System.Collections.Generic;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class OneProductOrder : LongIdentityModule
    {
        public const int BeginOrderNumber = 10000001;
        public const int PayOneProductType = 2;

        /// <summary>
        /// 商品Id
        /// </summary>
        public int ProductId = 0;
        /// <summary>
        /// 期数
        /// </summary>
        public long ProductNum = 0;
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId = 0;
        /// <summary>
        /// 订单号码
        /// </summary>
        public int OrderNum = 0;
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address = null;
        /// <summary>
        /// Ip地址
        /// </summary>
        [DataColumn(64)]
        public string Ip = null;
        /// <summary>
        /// Ip地址
        /// </summary>
        [DataColumn(64)]
        public string IpAddress = null;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ProductIdProductNum");
            DropIndex(ds, "ProductIdProductNumOrderNum");
            DropIndex(ds, "ProductIdProductNumUserId");
            DropIndex(ds, "UserId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ProductIdProductNum", "ProductId", "ProductNum");
            CreateIndex(ds, "ProductIdProductNumOrderNum", "ProductId", "ProductNum", "OrderNum");
            CreateIndex(ds, "ProductIdProductNumUserId", "ProductId", "ProductNum", "UserId");
            CreateIndex(ds, "UserId", "UserId");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (ProductId <= 0)
                return DataStatus.Failed;
            if (ProductNum <= 0L)
                return DataStatus.Failed;
            if (UserId <= 0L)
                return DataStatus.Failed;
            return base.OnInsertBefor(ds, mode, ref columns);
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        protected override void SetId(long id)
        {
            Id = id;
        }

        public static int GetCountByNumber(DataSource ds, int productId, long productNum, long orderId)
        {
            return (int)Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("Id", orderId, DbWhereType.LessThan) & W("ProductNum", productNum) & W("ProductId", productId))
                .Count();
        }
        public static int GetCountByUser(DataSource ds, int productId, long productNum, long userId)
        {
            return (int)Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("UserId", userId) & W("ProductNum", productNum) & W("ProductId", productId))
                .Count();
        }
        public static IList<OneProductOrder> GetTop(DataSource ds, DateTime now, int count)
        {
            return Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("CreationDate", now, DbWhereType.LessThanOrEqual))
                .OrderBy(D("CreationDate"))
                .ToList<OneProductOrder>(count);
        }
        public static IList<OneProductOrder> GetTopByNumberAndUser(DataSource ds, int productId, long productNum, int count)
        {
            return Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("ProductId", productId) & W("ProductNum", productNum))
                .OrderBy(D("CreationDate"))
                .ToList<OneProductOrder>(count);
        }
        public static OneProductOrder GetByOrder(DataSource ds, int productId, long productNum, int orderNum)
        {
            return Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("OrderNum", orderNum) & W("ProductId", productId) & W("ProductNum", productNum))
                .First<OneProductOrder>();
        }

        public static SplitPageData<OneProductOrder> GetPage(DataSource ds, int productId, long productNum, long index, int size, int show = 8)
        {
            long count;
            IList<OneProductOrder> list = Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("ProductId", productId) & W("ProductNum", productNum))
                .OrderBy(D("CreationDate"))
                .ToList<OneProductOrder>(size, index, out count);
            return new SplitPageData<OneProductOrder>(index, size, list, count, show);
        }

        public static IList<OneProductOrder> GetAllByUserProductNum(DataSource ds, long userId, int productId, long productNum)
        {
            return Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("ProductId", productId) & W("ProductNum", productNum) & W("UserId", userId))
                .OrderBy(D("CreationDate"))
                .ToList<OneProductOrder>();
        }

        public static int GetCount(DataSource ds, int productId, long productNum)
        {
            return (int)Db<OneProductOrder>.Query(ds)
                .Select()
                .Where(W("ProductId", productId) & W("ProductNum", productNum))
                .Count();
        }

        public static SplitPageData<DataJoin<OneProductOrder, OneProductNumber>> GetPageByUserAndState(DataSource ds, long userId, int state, long index, int size, int show = 8)
        {
            long count;
            IList<DataJoin<OneProductOrder, OneProductNumber>> list;
            switch (state)
            {
                case 1:
                    list = Db<OneProductOrder>.Query(ds)
                        .Select(S<OneProductOrder>(), S<OneProductNumber>(), S_AS<OneProduct>("Title"), S_AS<OneProduct>("Image"), S_AS<OneProduct>("Count"))
                        .InnerJoin(O<OneProductOrder>("ProductNum"), O<OneProductNumber>("Id")).And(O<OneProductOrder>("OrderNum"), O<OneProductNumber>("LuckNum"), DbJoinMode.NotEqual)
                        .InnerJoin(O<OneProductOrder>("ProductId"), O<OneProduct>("Id"))
                        .Where(W<OneProductOrder>("UserId", userId))
                        .OrderBy(D<OneProductOrder>("CreationDate"))
                        .ToList<DataJoin<OneProductOrder, OneProductNumber>>(size, index, out count);
                    break;
                case 2:
                    list = Db<OneProductOrder>.Query(ds)
                         .Select(S<OneProductOrder>(), S<OneProductNumber>(), S_AS<OneProduct>("Title"), S_AS<OneProduct>("Image"), S_AS<OneProduct>("Count"))
                         .InnerJoin(O<OneProductOrder>("ProductNum"), O<OneProductNumber>("Id")).And(O<OneProductOrder>("OrderNum"), O<OneProductNumber>("LuckNum"))
                         .InnerJoin(O<OneProductOrder>("ProductId"), O<OneProduct>("Id"))
                         .Where(W<OneProductOrder>("UserId", userId))
                         .OrderBy(D<OneProductOrder>("CreationDate"))
                         .ToList<DataJoin<OneProductOrder, OneProductNumber>>(size, index, out count);
                    break;
                default:
                    list = Db<OneProductOrder>.Query(ds)
                        .Select(S<OneProductOrder>(), S<OneProductNumber>(), S_AS<OneProduct>("Title"), S_AS<OneProduct>("Image"), S_AS<OneProduct>("Count"))
                        .InnerJoin(O<OneProductOrder>("ProductNum"), O<OneProductNumber>("Id"))
                        .InnerJoin(O<OneProductOrder>("ProductId"), O<OneProduct>("Id"))
                        .Where(W<OneProductOrder>("UserId", userId))
                        .OrderBy(D<OneProductOrder>("CreationDate"))
                        .ToList<DataJoin<OneProductOrder, OneProductNumber>>(size, index, out count);
                    break;
            }
            return new SplitPageData<DataJoin<OneProductOrder, OneProductNumber>>(index, size, list, count, show);
        }
    }
}
