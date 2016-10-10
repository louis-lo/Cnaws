using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using System.Text;
using Cnaws.ExtensionMethods;

namespace Cnaws.Pay.Modules
{
    [Serializable]
    public class PayRecord : NoIdentityModule, IOrder, IPayOrder, IRefundOrder
    {
        [DataColumn(true, 36)]
        public string Id = null;
        [DataColumn(true)]
        public PaymentType PayType = PaymentType.Pay;
        [DataColumn(32)]
        public string Provider = null;
        [DataColumn(36)]
        public string PayId = null;
        public long UserId = 0L;
        [DataColumn(36)]
        public string OpenId = null;
        [DataColumn(128)]
        public string Title = null;
        public int Type = 0;
        [DataColumn(64)]
        public string TargetId = null;
        public Money Money = 0;
        //public string Account = null;
        //public Money Fees = 0;
        public PayStatus Status = PayStatus.PayFailed;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        string IRefundOrder.PayProvider
        {
            get { return Provider; }
        }
        string IOrder.TradeNo
        {
            get { return Id; }
        }
        string IRefundOrder.PayTradeNo
        {
            get { return PayId; }
        }
        string IPayOrder.Subject
        {
            get { return Title; }
        }
        string IRefundOrder.Subject
        {
            get { return Title; }
        }
        Money IOrder.TotalFee
        {
            get { return Money; }
        }
        PayStatus IPayOrder.Status
        {
            get { return Status; }
        }
        string IPayOrder.OpenId
        {
            get { return OpenId; }
        }

        private static string NewId(DateTime time, long userId)
        {
            return string.Concat(time.ToTimestamp().ToString(), userId.ToString());
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "UserId");
            DropIndex(ds, "ProviderPayId");
            DropIndex(ds, "IdPayTypeStatus");
            DropIndex(ds, "IdUserIdPayTypeStatus");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "UserId", "UserId");
            CreateIndex(ds, "ProviderPayId", "Provider", "PayId");
            CreateIndex(ds, "IdPayTypeStatus", "Id", "PayType", "Status");
            CreateIndex(ds, "IdUserIdPayTypeStatus", "Id", "UserId", "PayType", "Status");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(Provider))
                return DataStatus.Failed;
            Provider = Provider.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            if (!string.IsNullOrEmpty(Provider))
                Provider = Provider.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }
        public string GetPayTypeName()
        {
            switch (Provider)
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
        public static PayRecord Create(DataSource ds, long user, string openId, string title, string provider, Money money, int type = 0, string targetId = null, PaymentType payType = PaymentType.Pay)
        {
            DateTime now = DateTime.Now;
            PayRecord pr = new PayRecord() { Id = NewId(now, user), PayType = payType, PayId = string.Empty, UserId = user, OpenId = openId, Title = title, Provider = provider, Money = money, Type = type, TargetId = targetId, Status = PayStatus.Paying, CreationDate = now };
            if (pr.Insert(ds) == DataStatus.Success)
                return pr;
            return null;
        }
        public static PayRecord GetById(DataSource ds, string id, PaymentType type)
        {
            return ExecuteSingleRow<PayRecord>(ds, P("PayType", type) & P("Id", id));
        }

        public static PayRecord GetByTypeAndStatusAndUserAndId(DataSource ds, string id, int type,long userid,PayStatus status)
        {
            return ExecuteSingleRow<PayRecord>(ds, P("Type", type) & P("Id", id) & P("Status", status)&P("UserId",userid));
        }
        public static PayRecord GetByRefund(DataSource ds, string id)
        {
            return ExecuteSingleRow<PayRecord>(ds, P("Status", PayStatus.RefundNotifying) & P("PayType", PaymentType.Refund) & P("Id", id));
        }
        public static PayRecord GetByUser(DataSource ds, string id, long userId, PaymentType type, PayStatus status)
        {
            return ExecuteSingleRow<PayRecord>(ds, P("PayType", type) & P("Status", status) & P("UserId", userId) & P("Id", id));
        }
        /// <summary>
        /// 根据用户查找支付记录
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static PayRecord GetByUser(DataSource ds, long userId, PaymentType pType, PayStatus status,int type)
        {
            return ExecuteSingleRow<PayRecord>(ds, 
                P("PayType", pType) &
                P("Type", type) &
                P("Status", status) & 
                P("UserId", userId));
        }
        public DataStatus UpdateStatus(DataSource ds, PayStatus old)
        {
            return Update(ds, ColumnMode.Include, Cs("Provider", "PayId", "Money", "Status"), P("PayType", PayType) & WN("Status", old, "Old") & P("Id", Id));
        }
    }
}
