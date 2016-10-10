using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;
using System.Collections.Generic;

namespace Cnaws.Product.Modules
{
    public enum OneProductNumberState
    {
        Normal = 0,
        Delivery = 1,
        Receipt = 2,
        Finished = 3
    }

    [Serializable]
    public sealed class OneProductNumber : LongIdentityModule
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int ProductId = 0;
        /// <summary>
        /// 状态
        /// </summary>
        public OneProductNumberState State = OneProductNumberState.Normal;
        /// <summary>
        /// 幸运号码
        /// </summary>
        public int LuckNum = 0;
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId = 0L;
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address = null;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime DeliveryDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 成交时间
        /// </summary>
        public DateTime ReceiptDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ProductId");
            DropIndex(ds, "State");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ProductId", "ProductId");
            CreateIndex(ds, "State", "State");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (ProductId <= 0)
                return DataStatus.Failed;
            return base.OnInsertBefor(ds, mode, ref columns);
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "ProductId", "CreationDate");
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static bool IsActive(DataSource ds, int productId)
        {
            return Db<OneProductNumber>.Query(ds)
                .Select()
                .Where(W("State", OneProductNumberState.Normal) & W("ProductId", productId))
                .Count() > 0;
        }
        public static DataStatus Create(DataSource ds, int productId)
        {
            OneProductNumber value = new OneProductNumber()
            {
                ProductId = productId,
                CreationDate = DateTime.Now
            };
            return value.Insert(ds);
        }

        public static OneProductNumber GetAllById(DataSource ds, long id)
        {
            return Db<OneProductNumber>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .First<OneProductNumber>();
        }
        public static IList<OneProductNumber> GetAllByProduct(DataSource ds, int productId)
        {
            return Db<OneProductNumber>.Query(ds)
                .Select()
                .Where(W("ProductId", productId))
                .OrderBy(D("Id"))
                .ToList<OneProductNumber>();
        }

        public static SplitPageData<DataJoin<OneProductNumber, OneProduct>> GetPage(DataSource ds, int state, long index, int size, int show = 8)
        {
            long count;
            IList<DataJoin<OneProductNumber, OneProduct>> list;
            if (state >= 0)
                list = Db<OneProductNumber>.Query(ds)
                .Select(S<OneProductNumber>(), S<OneProduct>("Title"), S<OneProduct>("Image"))
                .InnerJoin(O<OneProductNumber>("ProductId"), O<OneProduct>("Id"))
                .Where(W<OneProductNumber>("State", state))
                .OrderBy(D<OneProductNumber>("Id"))
                .ToList<DataJoin<OneProductNumber, OneProduct>>(size, index, out count);
            else if (state == -1)
                list = Db<OneProductNumber>.Query(ds)
                .Select(S<OneProductNumber>(), S<OneProduct>("Title"), S<OneProduct>("Image"))
                .InnerJoin(O<OneProductNumber>("ProductId"), O<OneProduct>("Id"))
                .OrderBy(D<OneProductNumber>("Id"))
                .ToList<DataJoin<OneProductNumber, OneProduct>>(size, index, out count);
            else
                throw new Exception();
            return new SplitPageData<DataJoin<OneProductNumber, OneProduct>>(index, size, list, count, show);
        }

        public static DataStatus ModState(DataSource ds, long id, OneProductNumberState state, long userid)
        {
            DataColumn[] dc = new DataColumn[3];
            dc[0] = new DataColumn("State");
            dc[1] = new DataColumn("DeliveryDate");
            dc[2] = new DataColumn("ReceiptDate");
            return (new OneProductNumber() { Id = id, State = OneProductNumberState.Finished, ReceiptDate = DateTime.Now, DeliveryDate = DateTime.Now }).Update(ds, ColumnMode.Include, dc, WN("State", OneProductNumberState.Delivery, "Old") & P("UserId", userid) & P("Id", id));
        }
    }
}
