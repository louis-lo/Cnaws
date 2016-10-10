using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Collections.Generic;
using Cnaws.Pay.Modules;
using Cnaws.ExtensionMethods;
using Cnaws.Pay;
using Cnaws.Json;
using Cnaws.Data.Query;
using System.ComponentModel;
using System.Linq;

namespace Cnaws.Product.Modules
{
#if (DEBUG)
    [Description("交易关闭\t等待完善\t等待付款\t等待发货\t等待收货\t等待评价\t交易完成\t申请退款\t等待退货发货\t退货已发货\t退款成功")]
#endif
    public enum OrderState
    {

        /// <summary>
        /// 交易关闭
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 等待完善
        /// </summary>
        Perfect = 1,
        /// <summary>
        /// 等待付款
        /// </summary>
        Payment = 2,
        /// <summary>
        /// 等待发货
        /// </summary>
        Delivery = 3,
        /// <summary>
        /// 等待收货
        /// </summary>
        Receipt = 4,
        /// <summary>
        /// 出库中
        /// </summary>
        OutWarehouse = 5,
        /// <summary>
        /// 交易完成
        /// </summary>
        Finished = 6,
        /// <summary>
        /// 申请退款
        /// </summary>
        PreRefund = 7,
        /// <summary>
        /// 等待退货发货
        /// </summary>
        PreReturn = 8,
        /// <summary>
        /// 退货已发货
        /// </summary>
        Return = 9,
        /// <summary>
        /// 退款成功
        /// </summary>
        Refund = 10,
    }
    public enum RefundType
    {
        /// <summary>
        /// 仅退款
        /// </summary>
        Refund = 0,
        /// <summary>
        /// 退货
        /// </summary>
        Return = 1
    }

    [Serializable]
    public class ProductOrder : NoIdentityModule
    {
        public const int PayProductType = 1;
        public const int PayWholesaleType = 5;

        public const int ChannelId = 1;

        /// <summary>
        /// 订单号
        /// </summary>
        [DataColumn(true, 36)]
        public string Id = null;
        /// <summary>
        /// 供应商Id
        /// </summary>
        public long SupplierId = 0L;
        /// <summary>
        /// 主订单Id号
        /// </summary>
        public string ParentId = null;
        /// <summary>
        /// 分销商Id
        /// </summary>
        public long ShopId = 0L;
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId = 0L;
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
        public long AddressId = 0L;
        [DataColumn(128)]
        public string Title = null;
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderState State = OrderState.Invalid;
        /// <summary>
        /// 支付方式
        /// </summary>
        [DataColumn(36)]
        public string Payment = null;
        /// <summary>
        /// 运费
        /// </summary>
        public Money FreightMoney = 0;
        /// <summary>
        /// 总金额
        /// </summary>
        public Money TotalMoney = 0;
        /// <summary>
        /// 收货地址
        /// </summary>
        [DataColumn(512)]
        public string Address = null;
        /// <summary>
        /// 买家留言
        /// </summary>
        [DataColumn(512)]
        public string Message = null;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime PaymentDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime DeliveryDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 成交时间
        /// </summary>
        public DateTime ReceiptDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundType RefundType = RefundType.Return;
        /// <summary>
        /// 退款原因
        /// </summary>
        public int RefundId = 0;
        /// <summary>
        /// 退款金额
        /// </summary>
        public Money RefundMoney = 0;
        /// <summary>
        /// 退款说明
        /// </summary>
        [DataColumn(512)]
        public string RefundSummary = null;
        /// <summary>
        /// 退款图片
        /// </summary>
        public string RefundImage = null;
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime PreRefundDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 退货发货时间
        /// </summary>
        public DateTime ReturnDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 退货成功时间（结算时间-有售后情况下会延长）
        /// </summary>
        public DateTime RefundDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        /// <summary>
        /// 1为普通订单、2为乡道馆
        /// </summary>
        public int Channel = ChannelId;
        ///// <summary>
        ///// 省Id
        ///// </summary>
        //public int Province = 0;
        ///// <summary>
        ///// 市Id
        ///// </summary>
        //public int City = 0;
        ///// <summary>
        ///// 区/县Id
        ///// </summary>
        //public int County = 0;

        public static string NewId(DateTime time, long userId, int index = 0)
        {
            if (index > 0)
                return string.Concat(time.AddSeconds(index).ToTimestamp().ToString(), userId.ToString());
            return string.Concat(time.ToTimestamp().ToString(), userId.ToString());
        }

        public PayRecord GetPayRecord(string payProvider, long userId, string openId)
        {
            return new PayRecord()
            {
                Id = Id,
                PayType = PaymentType.Pay,
                Provider = payProvider,
                PayId = string.Empty,
                UserId = userId,
                OpenId = openId,
                Title = Title,
                Type = PayProductType,
                Money = TotalMoney,
                TargetId = Id,
                Status = PayStatus.Paying,
                CreationDate = CreationDate
            };
        }

        public Money GetTotalMoney(DataSource ds)
        {
            return Db<ProductOrderMapping>.Query(ds).Select(S_SUM("TotalMoney")).Where(W("OrderId", Id)).First<ProductOrderMapping>().TotalMoney;
        }

        public string GetPayTypeName()
        {
            switch (Payment)
            {
                case "alipaydirect":
                    return "支付宝及时到账";
                case "wxpay":
                    return "微信支付";
                case "balance":
                    return "余额支付";
                case "alipaymobile":
                    return "支付宝移动支付";
                case "alipaygateway":
                    return "支付宝网关支付";
                case "alipayapp":
                    return "支付宝APP支付";
                case "alipayqr":
                    return "支付宝描码支付";
                case "cashondelivery":
                    return "货到付款";
                default:
                    return "未知方式";

            }
        }

        public static PayRecord GetPayRecord(string payProvider, string orderId, long userId, string title, Money money, string openId, int type = PayProductType)
        {
            return new PayRecord()
            {
                Id = orderId,
                PayType = PaymentType.Pay,
                Provider = payProvider,
                PayId = string.Empty,
                UserId = userId,
                OpenId = openId,
                Title = title,
                Type = type,
                Money = money,
                TargetId = orderId,
                Status = PayStatus.Paying,
                CreationDate = DateTime.Now
            };
        }
        public string GetStateInfo()
        {
            switch (State)
            {
                case OrderState.OutWarehouse: return "出库中";
                case OrderState.Invalid: return "交易关闭";
                case OrderState.Perfect: return "等待付款";
                case OrderState.Payment: return "等待付款";
                case OrderState.Delivery: return "等待发货";
                case OrderState.Receipt: return "等待收货";
                //case OrderState.Evaluation: return "等待评价";
                case OrderState.Refund:
                case OrderState.Finished: return "交易成功";
                case OrderState.PreRefund: return "已申请退款";
                case OrderState.PreReturn: return "等待买家退货";
                case OrderState.Return: return "等待卖家收货";
            }
            return "错误的状态";
        }
        public string GetStateString()
        {
            switch (State)
            {
                case OrderState.Delivery: return "付款";
                case OrderState.Receipt: return "发货";
                //case OrderState.Evaluation:
                case OrderState.Finished: return "收货";
                default: return "创建";
            }
        }
        public DateTime GetDateTime()
        {
            switch (State)
            {
                case OrderState.Delivery: return PaymentDate;
                case OrderState.Receipt: return DeliveryDate;
                //case OrderState.Evaluation:
                case OrderState.Finished: return ReceiptDate;
                default: return CreationDate;
            }
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "SupplierId");
            DropIndex(ds, "ShopId");
            DropIndex(ds, "UserId");
            DropIndex(ds, "State");
            DropIndex(ds, "SupplierIdState");
            DropIndex(ds, "ShopIdState");
            DropIndex(ds, "UserIdState");
            DropIndex(ds, "ParentId");
            DropIndex(ds, "ParentIdState");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "SupplierId", "SupplierId");
            CreateIndex(ds, "ShopId", "ShopId");
            CreateIndex(ds, "UserId", "UserId");
            CreateIndex(ds, "State", "State");
            CreateIndex(ds, "SupplierIdState", "SupplierId", "State");
            CreateIndex(ds, "ShopIdState", "ShopId", "State");
            CreateIndex(ds, "UserIdState", "UserId", "State");
            CreateIndex(ds, "ParentId", "ParentId");
            CreateIndex(ds, "ParentIdState", "ParentId", "State");
        }

        public IList<ProductOrderMapping> GetMapping(DataSource ds)
        {
            return ProductOrderMapping.GetAllByOrder(ds, Id);
        }
        public IList<ProductOrder> GetChildOrders(DataSource ds)
        {
            return DataQuery
               .Select<ProductOrder>(ds)
               .Where(P("UserId", UserId) & P("ParentId", Id))
               .ToList<ProductOrder>();
        }
        //public static Money GetFreight(DataSource ds, string orderId, int provice, int city, Money total)
        //{
        //    Money money = 0;
        //    IList<Product> ps = Db<Product>.Query(ds)
        //        .Select()
        //        .Where(W("Id").InSelect<ProductOrderMapping>("ProductId").Where(W("OrderId", orderId)).Result())
        //        .ToList<Product>();
        //    for (int i = 0; i < ps.Count; ++i)
        //    {
        //        if (i == 0)
        //            money = ps[i].GetFreight(ds, provice, city, total);
        //        else
        //            money = new Money(Math.Min(money, ps[i].GetFreight(ds, provice, city, total)));
        //    }
        //    return money;
        //}
        /// <summary>
        /// 查询运费
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="orderId"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public new Money GetFreight(DataSource ds, int provice, int city)
        {
            Money money = 0;
            bool IsActivityFree = false;
            if (Channel == 1)
            {
                Supplier supplier = Supplier.GetById(ds, SupplierId);
                if (supplier.IsActivityFree)
                {
                    if (TotalMoney >= supplier.ActivityCondition)
                    {
                        money = supplier.ActivityFree;
                        IsActivityFree = true;
                    }
                }
            }
            if (!IsActivityFree)
            {
                IList<ProductOrderMapping> mappings = ProductOrderMapping.GetAllByOrder(ds, Id);
                IList<Product> ps = Db<Product>.Query(ds)
                    .Select()
                    .Where(W("Id", mappings.Select(x => x.ProductId).ToArray(), DbWhereType.In))
                    .ToList<Product>();

                long[] template = ps.Where(w => w.FreightType == Cnaws.Product.Modules.FreightType.Template).GroupBy(x => x.FreightTemplate, a => a.FreightTemplate).Select(s => s.Key).ToArray();

                int Number = mappings.Select(s => s.Count).Sum();
                int Volume = 0, Weight = 0;
                foreach (ProductOrderMapping mapping in mappings)
                {
                    Product product = ps.Where(x => x.Id == mapping.ProductId).First();
                    Volume += (product.Volume * mapping.Count);
                    Weight += (product.Weight * mapping.Count);
                }
                if (template.Length > 0)
                {
                    for (int i = 0; i < template.Length; i++)
                    {
                        if (i == 0)
                            money = Cnaws.Product.Modules.FreightTemplate.GetFreight(ds, template[i], provice, city, 0, TotalMoney, Number, Volume, Weight);
                        else
                            money = new Money(Math.Max(money, Cnaws.Product.Modules.FreightTemplate.GetFreight(ds, template[i], provice, city, 0, TotalMoney, Number, Volume, Weight)));
                    }
                }
                else
                {
                    for (int i = 0; i < ps.Count(); i++)
                    {
                        if (i == 0)
                            money = ps[i].FreightMoney;
                        else
                            money = new Money(Math.Max(money, ps[i].FreightMoney));
                    }
                }
            }
            return money;
        }

        public static new Money GetStaticFreight(DataSource ds,string orderid,long supplierid, int provice, int city,Money total)
        {
            ProductOrder order = GetById(ds, orderid);
            Money money = 0;
            bool IsActivityFree = false;
            if (order.Channel == 1)
            {
                Supplier supplier = Supplier.GetById(ds, supplierid);
                if (supplier.IsActivityFree)
                {
                    if (total >= supplier.ActivityCondition)
                    {
                        money = supplier.ActivityFree;
                        IsActivityFree = true;
                    }
                }
            }
            if (!IsActivityFree)
            {
                IList<ProductOrderMapping> mappings = ProductOrderMapping.GetAllByOrder(ds, orderid);
                IList<Product> ps = Db<Product>.Query(ds)
                    .Select()
                    .Where(W("Id", mappings.Select(x => x.ProductId).ToArray(), DbWhereType.In))
                    .ToList<Product>();

                long[] template = ps.Where(w => w.FreightType == Cnaws.Product.Modules.FreightType.Template).GroupBy(x => x.FreightTemplate, a => a.FreightTemplate).Select(s => s.Key).ToArray();

                int Number = mappings.Select(s => s.Count).Sum();
                int Volume = 0, Weight = 0;
                foreach (ProductOrderMapping mapping in mappings)
                {
                    Product product = ps.Where(x => x.Id == mapping.ProductId).First();
                    Volume += (product.Volume * mapping.Count);
                    Weight += (product.Weight * mapping.Count);
                }
                if (template.Length > 0)
                {
                    for (int i = 0; i < template.Length; i++)
                    {
                        if (i == 0)
                            money = Cnaws.Product.Modules.FreightTemplate.GetFreight(ds, template[i], provice, city, 0, total, Number, Volume, Weight);
                        else
                            money = new Money(Math.Max(money, Cnaws.Product.Modules.FreightTemplate.GetFreight(ds, template[i], provice, city, 0, total, Number, Volume, Weight)));
                    }
                }
                else
                {
                    for (int i = 0; i < ps.Count(); i++)
                    {
                        if (i == 0)
                            money = ps[i].FreightMoney;
                        else
                            money = new Money(Math.Max(money, ps[i].FreightMoney));
                    }
                }
            }
            return money;
        }
        public Supplier GetSupplier(DataSource ds)
        {
            if (Channel == 1)
                return Supplier.GetById(ds, SupplierId);
            else
            {
                Distributor distributor = Distributor.GetById(ds, SupplierId);
                if (distributor != null)
                    return new Supplier()
                    {
                        UserId = distributor.UserId,
                        Address = distributor.Address,
                        Company = distributor.Company,
                        Contact = distributor.Contact,
                        ContactPhone = distributor.ContactPhone,
                        CreationDate = distributor.CreationDate,
                        Images = distributor.Images,
                        Level = 0
                    };
                else return null;
            }
        }

        public string GetStoreName(DataSource ds)
        {
            string name = StoreInfo.GetStoreInfoByUserId(ds, SupplierId)?.StoreName;
            if (string.IsNullOrEmpty(name))
                name = GetSupplier(ds)?.Company;
            return name;
        }

        /// <summary>
        /// 自动确认收货
        /// </summary>
        /// <param name="ds"></param>
        internal static void AutomaticReceipt(DataSource ds)
        {
            Db<ProductOrder>.Query(ds).Update()
                .Set("State", OrderState.Finished)
                .Set("ReceiptDate", DateTime.Now)
                .Set("RefundDate", DateTime.Now)
                .Where(W("State", OrderState.Receipt) & W("DeliveryDate", DateTime.Now.AddDays(-15), DbWhereType.LessThan))
                .Execute();
        }

        public long GetCount(DataSource ds)
        {
            DbWhereQueue dw = W("OrderId", Id);
            return Db<ProductOrderMapping>.Query(ds)
                    .Select()
                    .Where(dw)
                    .Count();
        }
        public static ProductOrder GetById(DataSource ds, string id)
        {
            return Db<ProductOrder>.Query(ds)
                .Select()
                .Where(W("Id", id))
                .First<ProductOrder>();
        }

        public static IList<ProductOrder> GetByIds(DataSource ds, string[] ids)
        {
            return Db<ProductOrder>.Query(ds)
                .Select()
                .Where(W("Id", ids, DbWhereType.In))
                .ToList<ProductOrder>();
        }
        public static ProductOrder GetByState(DataSource ds, string id, OrderState state, long userId)
        {
            AutomaticReceipt(ds);
            return DataQuery
                .Select<ProductOrder>(ds)
                .Where(P("State", state) & P("UserId", userId) & P("Id", id))
                .First<ProductOrder>();
        }
        public static IList<ProductOrder> GetListByState(DataSource ds, string parentid, OrderState state, long userId)
        {
            AutomaticReceipt(ds);
            return DataQuery
                .Select<ProductOrder>(ds)
                .Where(P("State", state) & P("UserId", userId) & P("ParentId", parentid))
                .ToList<ProductOrder>();
        }
        public static IList<ProductOrder> GetListByParentid(DataSource ds, string parentid, long userId)
        {
            AutomaticReceipt(ds);
            return DataQuery
                .Select<ProductOrder>(ds)
                .Where(P("UserId", userId) & P("ParentId", parentid))
                .ToList<ProductOrder>();
        }

        public static ProductOrder GetByUser(DataSource ds, string id, long userId)
        {
            AutomaticReceipt(ds);
            return DataQuery
                .Select<ProductOrder>(ds)
                .Where(P("UserId", userId) & P("Id", id))
                .First<ProductOrder>();
        }

        public DataStatus UpdatePerfectByUser(DataSource ds)
        {
            return Update(ds, ColumnMode.Include, Cs("State", "FreightMoney", "TotalMoney", "Address", "Message", "Province", "City", "County"), WN("State", OrderState.Perfect, "Value") & P("UserId", UserId) & P("Id", Id));
        }

        public DataStatus UpdateFreightBySupplier(DataSource ds)
        {
            //修改邮费并修改订单号
            ds.Begin();
            try
            {
                ProductOrder order = GetById(ds, Id);
                if (Update(ds, ColumnMode.Include, Cs(MODC("FreightMoney", FreightMoney-order.FreightMoney), MODC("TotalMoney", FreightMoney - order.FreightMoney)), WN("FreightMoney", -(FreightMoney - order.FreightMoney), "Value", ">=") & P("State", OrderState.Payment) & P("SupplierId", SupplierId) & P("Id", Id)) == DataStatus.Success)
                {
                    string NewOrderId = NewId(DateTime.Now, order.UserId);
                    if (Db<ProductOrder>.Query(ds).Update().Set("Id", NewOrderId).Where(W("Id", Id)).Execute() <= 0)
                    {
                        throw new Exception();
                    }
                    if (Db<ProductOrderMapping>.Query(ds).Update().Set("OrderId", NewOrderId).Where(W("OrderId", Id)).Execute() <= 0)
                    {
                        throw new Exception();
                    }
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
                return DataStatus.Rollback;
            }

        }

        internal DataStatus UpdateState(DataSource ds, OrderState state)
        {//出库中
            switch (state)
            {
                case OrderState.Payment:
                    PaymentDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Delivery;
                    return Update(ds, ColumnMode.Include, Cs("State", "PaymentDate", "RefundDate"), WN("State", state, "Value") & P("Id", Id));
                case OrderState.Delivery:
                    RefundDate = DateTime.Now;
                    State = OrderState.OutWarehouse;
                    return Update(ds, ColumnMode.Include, Cs("State", "RefundDate"), WN("State", state, "Value") & P("Id", Id));
                case OrderState.OutWarehouse:
                    DeliveryDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Delivery;
                    return Update(ds, ColumnMode.Include, Cs("State", "DeliveryDate", "RefundDate"), WN("State", state, "Value") & P("Id", Id));
                case OrderState.Receipt:
                    ReceiptDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Finished;
                    return Update(ds, ColumnMode.Include, Cs("State", "ReceiptDate", "RefundDate"), WN("State", state, "Value") & P("Id", Id));
                //case OrderState.Evaluation:
                //    State = OrderState.Finished;
                //    return Update(ds, ColumnMode.Include, Cs("State"), WN("State", state, "Value") & P("Id", Id));
                default:
                    return DataStatus.Failed;
            }
        }
        public DataStatus UpdateStateByUser(DataSource ds, OrderState state,string payment="")
        {
            switch (state)
            {
                case OrderState.Payment:
                    PaymentDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(payment))
                    Payment = payment.ToLower();
                    State = OrderState.Delivery;
                    return Update(ds, ColumnMode.Include, Cs("State", "Payment", "PaymentDate", "RefundDate"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                case OrderState.Delivery:
                    RefundDate = DateTime.Now;
                    State = OrderState.OutWarehouse;
                    return Update(ds, ColumnMode.Include, Cs("State", "RefundDate"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                case OrderState.OutWarehouse:
                    DeliveryDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Receipt;
                    return Update(ds, ColumnMode.Include, Cs("State", "DeliveryDate", "RefundDate"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                case OrderState.Receipt:
                    ReceiptDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Finished;
                    DataStatus status = Update(ds, ColumnMode.Include, Cs("State", "ReceiptDate", "RefundDate"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                    return status;
                //case OrderState.Evaluation:
                //    State = OrderState.Finished;
                //    return Update(ds, ColumnMode.Include, Cs("State"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                default:
                    return DataStatus.Failed;
            }
        }
        public DataStatus UpdateStateByUserNoState(DataSource ds, OrderState state)
        {
            switch (state)
            {
                case OrderState.Payment:
                    PaymentDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Delivery;
                    return Update(ds, ColumnMode.Include, Cs("State", "PaymentDate", "RefundDate"),P("UserId", UserId) & P("Id", Id));
                case OrderState.Delivery:
                    RefundDate = DateTime.Now;
                    State = OrderState.OutWarehouse;
                    return Update(ds, ColumnMode.Include, Cs("State", "RefundDate"),  P("UserId", UserId) & P("Id", Id));
                case OrderState.OutWarehouse:
                    DeliveryDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Receipt;
                    return Update(ds, ColumnMode.Include, Cs("State", "DeliveryDate", "RefundDate"),  P("UserId", UserId) & P("Id", Id));
                case OrderState.Receipt:
                    ReceiptDate = DateTime.Now;
                    RefundDate = DateTime.Now;
                    State = OrderState.Finished;
                    DataStatus status = Update(ds, ColumnMode.Include, Cs("State", "ReceiptDate", "RefundDate"),  P("UserId", UserId) & P("Id", Id));
                    return status;
                //case OrderState.Evaluation:
                //    State = OrderState.Finished;
                //    return Update(ds, ColumnMode.Include, Cs("State"), WN("State", state, "Value") & P("UserId", UserId) & P("Id", Id));
                default:
                    return DataStatus.Failed;
            }
        }

        public static dynamic GetAllProductInfo(DataSource ds, long userId)
        {

            long all = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(W("UserId", userId) & W("State", OrderState.Perfect, DbWhereType.NotEqual))
                .Count();
            long payment = DataQuery.Select<ProductOrder>(ds)
                .Where(WN("State", OrderState.Payment, "Payment") & P("UserId", userId))
                .Count();
            long dlivery = DataQuery.Select<ProductOrder>(ds)
                .Where(P("State", OrderState.Delivery) & P("UserId", userId))
                .Count();
            long warehouse = DataQuery.Select<ProductOrder>(ds)
                .Where(P("State", OrderState.OutWarehouse) & P("UserId", userId))
                .Count();
            long receipt = DataQuery.Select<ProductOrder>(ds)
                .Where(P("State", OrderState.Receipt) & P("UserId", userId))
                .Count();
            long finished = DataQuery.Select<ProductOrder>(ds)
                .Where(P("State", OrderState.Finished) & P("UserId", userId))
                .Count();
            //long evaluation = DataQuery.Select<ProductOrder>(ds)
            //    .Where(P("State", OrderState.Evaluation) & P("UserId", userId))
            //    .Count();
            return new
            {
                All = all,
                Payment = payment,
                Delivery = dlivery,
                OutWarehouse = warehouse,//出库中
                Receipt = receipt,
                Finished = finished
                //Evaluation = evaluation
            };
        }
        public static SplitPageData<ProductOrder> GetPageByUser(DataSource ds, long userId, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(W("UserId", userId) & W("State", OrderState.Perfect, DbWhereType.NotEqual))
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            return new SplitPageData<ProductOrder>(index, size, list, count, show);
        }
        public static SplitPageData<ProductOrder> GetPageByUserAndState(DataSource ds, long userId, string state, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue dw = W("UserId", userId);
            if (!string.IsNullOrEmpty(state) && !"_".Equals(state))
            {
                OrderState os = (OrderState)Enum.Parse(TType<OrderState>.Type, state, true);
                if (os == OrderState.Payment)
                    dw &= W("State", OrderState.Payment);
                else
                    dw &= W("State", os);
            }
            dw &= W("State", OrderState.Perfect, DbWhereType.NotEqual);
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(dw)
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            return new SplitPageData<ProductOrder>(index, size, list, count, show);
        }
        public static SplitPageData<dynamic> GetAjaxPageByUserAndState(DataSource ds, long userId, string state, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue dw = W("UserId", userId);
            if (!string.IsNullOrEmpty(state) && !"_".Equals(state))
            {
                OrderState os = (OrderState)Enum.Parse(TType<OrderState>.Type, state, true);
                if (os == OrderState.Payment)
                    dw &= W("State", OrderState.Payment);
                else
                    dw &= W("State", os);
            }
            dw &= W("State", OrderState.Perfect, DbWhereType.NotEqual);
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(dw)
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            List<dynamic> temp;
            ProductCacheInfo pci;
            IList<ProductOrderMapping> maps;
            List<dynamic> result = new List<dynamic>(list.Count);
            foreach (ProductOrder item in list)
            {
                maps = item.GetMapping(ds);
                temp = new List<dynamic>(maps.Count);
                foreach (ProductOrderMapping it in maps)
                {
                    pci = JsonValue.Deserialize<ProductCacheInfo>(it.ProductInfo);
                    temp.Add(new
                    {
                        ProductId = it.ProductId,
                        Image = it.GetImage(pci.Image),
                        ProductInfo = pci,
                        Price = it.Price,
                        TotalMoney = it.TotalMoney,
                        Count = it.Count,
                        Evaluation = it.Evaluation,
                        IsService = it.IsService,
                        AfterSalesOrderId = it.AfterSalesOrderId
                    });
                }
                result.Add(new
                {
                    Id = item.Id,
                    State = (int)item.State,
                    StateInfo = item.GetStateInfo(),
                    Mappings = temp,
                    FreightMoney = item.FreightMoney,
                    TotalMoney = item.TotalMoney,
                    CreationDate = item.CreationDate,
                    Address = item.Address,
                    Message = item.Message,
                });
            }
            return new SplitPageData<dynamic>(index, size, result, count, show);
        }

        public static SplitPageData<ProductOrder> GetPageBySupplier(DataSource ds, long userId, int state,string title,string nickName,string startDate,string endDate, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue where;
            if (state == (int)OrderState.Delivery)
                where = W("State", OrderState.Delivery) | W("State", OrderState.OutWarehouse);
            else if (state > -1)
                where = W("State", state);
            else
                where = W("State", OrderState.Receipt);/* | W("State", OrderState.Evaluation)*/
            where &= W("SupplierId", userId);//增加频道

            if (!string.IsNullOrEmpty(title))
            {
                where &= (W("Id", title, DbWhereType.Like)|W("Id").InSelect<ProductOrderMapping>(S("OrderId")).Where(W("ProductTitle",title, DbWhereType.Like)).Result());
            }

            if (!string.IsNullOrEmpty((string)nickName))
            {
                where &= W("Address", nickName, DbWhereType.Like);
            }
            string DateType = "CreationDate";
            if (state == (int)OrderState.Delivery||state== (int)OrderState.OutWarehouse)
                DateType = "PaymentDate";
            else if(state == (int)OrderState.Finished)
                DateType = "ReceiptDate";
            else if (state == (int)OrderState.Receipt)
                DateType = "DeliveryDate";

            DateTime begindt = new DateTime(), endtime = new DateTime();
            if (DateTime.TryParse((string)startDate, out begindt) && DateTime.TryParse((string)endDate, out endtime))
            {
                where &= (W(DateType, begindt, DbWhereType.GreaterThan) & W(DateType, endtime, DbWhereType.LessThan));
            }
            else if (DateTime.TryParse((string)startDate, out begindt))
            {
                where &= W(DateType, begindt, DbWhereType.GreaterThan);
            }
            else if (DateTime.TryParse((string)endDate, out endtime))
            {
                where &= W(DateType, endtime, DbWhereType.LessThan);
            }

            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(D("Id"))
                .ToList<ProductOrder>(size, index, out count);
            return new SplitPageData<ProductOrder>(index, size, list, count, show);
        }


        public static SplitPageData<ProductOrder> GetPageByDistributor(DataSource ds, long userId, int state, string title, string nickName, string startDate, string endDate, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue where;
            if (state == (int)OrderState.Delivery)
                where = W("State", OrderState.Delivery) | W("State", OrderState.OutWarehouse);
            else if (state > -1)
                where = W("State", state);
            else
                where = W("State", OrderState.Receipt);/* | W("State", OrderState.Evaluation)*/
            where &= (W("SupplierId").InSelect<Supplier>(S("UserId")).Where(W("Subjection",userId)).Result());

            if (!string.IsNullOrEmpty(title))
            {
                where &= (W("Id", title, DbWhereType.Like) | W("Id").InSelect<ProductOrderMapping>(S("OrderId")).Where(W("ProductTitle", title, DbWhereType.Like)).Result());
            }

            if (!string.IsNullOrEmpty((string)nickName))
            {
                where &= W("Address", nickName, DbWhereType.Like);
            }
            string DateType = "CreationDate";
            if (state == (int)OrderState.Delivery || state == (int)OrderState.OutWarehouse)
                DateType = "PaymentDate";
            else if (state == (int)OrderState.Finished)
                DateType = "ReceiptDate";
            else if (state == (int)OrderState.Receipt)
                DateType = "DeliveryDate";

            DateTime begindt = new DateTime(), endtime = new DateTime();
            if (DateTime.TryParse((string)startDate, out begindt) && DateTime.TryParse((string)endDate, out endtime))
            {
                where &= (W(DateType, begindt, DbWhereType.GreaterThan) & W(DateType, endtime, DbWhereType.LessThan));
            }
            else if (DateTime.TryParse((string)startDate, out begindt))
            {
                where &= W(DateType, begindt, DbWhereType.GreaterThan);
            }
            else if (DateTime.TryParse((string)endDate, out endtime))
            {
                where &= W(DateType, endtime, DbWhereType.LessThan);
            }

            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(where)
                .OrderBy(D("Id"))
                .ToList<ProductOrder>(size, index, out count);
            return new SplitPageData<ProductOrder>(index, size, list, count, show);
        }

        public static SplitPageData<DataJoin<ProductOrder, Supplier>> GetPage(DataSource ds, int state, int type, string orderid, long index, int size, int show, int channel = 1)
        {
            AutomaticReceipt(ds);
            long count;
            IList<DataJoin<ProductOrder, Supplier>> list;
            DbWhereQueue where = null;
            DbOrderBy[] order = new DbOrderBy[1];
            switch (state)
            {
                case -1:
                    where = new DbWhereQueue();
                    order[0] = D<ProductOrder>("Id");
                    break;
                case (int)OrderState.Delivery:
                    where = W<ProductOrder>("State", state);
                    order[0] = D<ProductOrder>("PaymentDate");
                    break;
                case (int)OrderState.OutWarehouse:
                    where = W<ProductOrder>("State", state);
                    order[0] = D<ProductOrder>("PaymentDate");
                    break;
                case (int)OrderState.Receipt:
                    where = W<ProductOrder>("State", state);
                    order[0] = D<ProductOrder>("DeliveryDate");
                    break;
                //case (int)OrderState.Evaluation:
                case (int)OrderState.Finished:
                    where = W<ProductOrder>("State", state);
                    order[0] = D<ProductOrder>("ReceiptDate");
                    break;
                case (int)OrderState.Invalid:
                case (int)OrderState.Perfect:
                case (int)OrderState.Payment:
                    where = W<ProductOrder>("State", state);
                    order[0] = D<ProductOrder>("CreationDate");
                    break;
                default:
                    throw new Exception();
            }
            if (state != -1 && !string.IsNullOrEmpty(orderid))
            {
                where &= W<ProductOrder>("Id", orderid);
            }
            else if (!string.IsNullOrEmpty(orderid))
            {
                where = W<ProductOrder>("Id", orderid);
            }
            where &= W<ProductOrder>("Channel", channel);//增加频道
            list = Db<ProductOrder>.Query(ds)
                .Select(S<ProductOrder>(), S<Supplier>())
                .InnerJoin(O<ProductOrder>("SupplierId"), O<Supplier>("UserId"))
                .Where(where)
                .OrderBy(D<ProductOrder>("DeliveryDate"), D<ProductOrder>("ReceiptDate"), D<ProductOrder>("PaymentDate"), D<ProductOrder>("CreationDate"))
                .ToList<DataJoin<ProductOrder, Supplier>>(size, index, out count);
            return new SplitPageData<DataJoin<ProductOrder, Supplier>>(index, size, list, count, show);
        }

        public static long GetCountByState(DataSource ds, OrderState state, long userId, int channel = 1)
        {
            return Db<ProductOrderMapping>.Query(ds)
                    .Select()
                    .Where(W("OrderId").InSelect<ProductOrder>("Id").Where(W("State", state) & W("SupplierId", userId) & W("Channel", channel)).Result())//增加频道
                    .Count();
        }

        public static long GetMyCountByState(DataSource ds, OrderState state, long userId)
        {
            DbWhereQueue dw = W("UserId", userId);
            if (state == OrderState.Payment)
                dw &= W("State", OrderState.Payment);
            else
                dw &= W("State", state);
            return Db<ProductOrder>.Query(ds)
                    .Select()
                    .Where(dw)
                    .Count();
        }

        public bool IsCanAddService()
        {
            if (State == OrderState.Finished)
            {
                return RefundDate.AddDays(7) >= DateTime.Now ? true : false;
            }
            else
            {
                return true;
            }

        }

        public static DataStatus ModOrder(DataSource ds, ProductOrder order)
        {
            return order.Update(ds);
        }

        public static DataStatus DelOrder(DataSource ds, string orderid)
        {
            if (Db<ProductOrder>.Query(ds).Delete().Where(W("Id", orderid)).Execute() > 0)
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

        /// <summary>
        /// 获取订单状态字符
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetStateText(OrderState state)
        {
            string stateTxt = string.Empty;
            switch (state)
            {
                case OrderState.Invalid:
                    stateTxt = "交易关闭";
                    break;
                case OrderState.Perfect:
                    stateTxt = "等待完善";
                    break;
                case OrderState.Payment:
                    stateTxt = "等待付款";
                    break;
                case OrderState.Delivery:
                    stateTxt = "等待发货";
                    break;
                case OrderState.OutWarehouse:
                    stateTxt = "出库中";
                    break;
                case OrderState.Receipt:
                    stateTxt = "等待收货";
                    break;
                case OrderState.Finished:
                    stateTxt = "交易完成";
                    break;
                default:
                    stateTxt = "未知状态";
                    break;
            }
            return stateTxt;
        }
        /// <summary>
        /// 修改订单的父订单号
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="orderid"></param>
        /// <param name="neworderid"></param>
        /// <returns></returns>
        public static DataStatus UpdateParentId(DataSource ds, string orderid, string neworderid)
        {
            if (Db<ProductOrder>.Query(ds).Update().Set("ParentId", neworderid).Where(W("Id", orderid)).Execute() > 0)
                return DataStatus.Success;
            else
                return DataStatus.Failed;
        }

        #region Api专用方法
        [Obsolete]
        public static SplitPageData<dynamic> GetAjaxPageByUserAndStateApi(DataSource ds, long userId, string state, long index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue dw = W("UserId", userId);//增加频道
            if (!string.IsNullOrEmpty(state) && !"_".Equals(state))
            {
                OrderState os = (OrderState)Enum.Parse(TType<OrderState>.Type, state, true);
                if (os == OrderState.Payment)
                    dw &= W("State", OrderState.Payment);
                else
                    dw &= W("State", os);
            }
            dw &= W("State", OrderState.Perfect, DbWhereType.NotEqual);
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(dw)
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            List<dynamic> temp;
            ProductCacheInfo pci;
            IList<ProductOrderMapping> maps;
            List<dynamic> result = new List<dynamic>(list.Count);
            foreach (ProductOrder item in list)
            {
                maps = item.GetMapping(ds);
                temp = new List<dynamic>(maps.Count);
                foreach (ProductOrderMapping it in maps)
                {
                    pci = JsonValue.Deserialize<ProductCacheInfo>(it.ProductInfo);
                    temp.Add(new
                    {
                        ProductId = it.ProductId,
                        Image = it.GetImage(pci.Image),
                        ProductInfo = pci,
                        Price = it.Price,
                        TotalMoney = it.TotalMoney,
                        Count = it.Count,
                        Evaluation = it.Evaluation,
                        IsService = it.IsService,
                        AfterSalesOrderId = it.AfterSalesOrderId
                    });
                }
                result.Add(new
                {
                    Id = item.Id,
                    State = (int)item.State,
                    StateInfo = item.GetStateInfo(),
                    CreationDate = item.CreationDate,
                    Address = item.Address,
                    Message = item.Message,
                    Mappings = temp,
                    FreightMoney = item.FreightMoney,
                    TotalMoney = item.TotalMoney
                });
            }
            return new SplitPageData<dynamic>(index, size, result, count, show);
        }

        /// <summary>
        /// 2.0版本根据用户获取订单列表(包含产品)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public static SplitPageData<dynamic> GetAjaxPageByUserAndStateApi2(DataSource ds, long userId, string state, int index, int size, int show)
        {
            AutomaticReceipt(ds);
            long count;
            DbWhereQueue dw = W("UserId", userId);
            if (!string.IsNullOrEmpty(state) && !"_".Equals(state))
            {
                OrderState os = (OrderState)Enum.Parse(TType<OrderState>.Type, state, true);
                if (os == OrderState.Payment)
                    dw &= W("State", OrderState.Payment);
                else
                    dw &= W("State", os);
            }
            dw &= W("State", OrderState.Perfect, DbWhereType.NotEqual);
            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(dw)
                .OrderBy(D("CreationDate"))
                .ToList<ProductOrder>(size, index, out count);
            ProductCacheInfo pci;
            IList<ProductOrderMapping> maps;
            List<dynamic> result = new List<dynamic>();
            foreach (ProductOrder item in list)
            {
                maps = item.GetMapping(ds);
                List<OrderMappingCacheInfo> temp = new List<OrderMappingCacheInfo>(maps.Count);
                foreach (ProductOrderMapping it in maps)
                {
                    temp.Add(new OrderMappingCacheInfo(it));
                }
                result.Add(new
                {
                    Order = item,
                    Products = temp,
                });
            }
            return new SplitPageData<dynamic>(index, size, result, count, show);
        }

        #endregion Api专用方法

        /// <summary>
        /// 根据订单号，商品名称，下单时间以及状态搜索
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="show"></param>
        /// <param name="queryText"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static new SplitPageData<ProductOrder> SearchOrder(
            DataSource ds,
            long userId,
            string state,
            long index,
            int size,
            int show,
            string queryText,
            string startDate,
            string endDate,
            int channel = 2)
        {
            long count;
            DbWhereQueue dw = W("UserId", userId) & W("Channel", channel);
            if (!string.IsNullOrEmpty(state) && !"_".Equals(state))
            {
                OrderState os = (OrderState)Enum.Parse(TType<OrderState>.Type, state, true);
                dw &= (W("State", (int)os));
            }
            if (!string.IsNullOrEmpty(queryText))
            {
                long orderId = 0;
                long.TryParse(queryText, out orderId);
                if (orderId > 0)
                {
                    dw &= W("Id", orderId, DbWhereType.LikeBegin);
                }
                else
                {
                    dw &= W("Id")
                        .InSelect<ProductOrderMapping>(S("OrderId")).Where(
                        W("ProductId").InSelect<Product>(S("Id")).Where(W("Title", queryText, DbWhereType.Like)).Result()
                        ).Result();
                }
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                dw &= W("CreationDate", startDate, DbWhereType.GreaterThanOrEqual);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                dw &= W("CreationDate", endDate, DbWhereType.LessThanOrEqual);
            }

            IList<ProductOrder> list = Db<ProductOrder>.Query(ds)
                .Select()
                .Where(dw)
                .OrderBy(new DbOrderBy("CreationDate", DbOrderByType.Desc))
                .ToList<ProductOrder>(size, index, out count);
            return new SplitPageData<ProductOrder>(index, size, list, count, show);
        }
    }

}

