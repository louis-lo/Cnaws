using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Passport.Modules
{
    /// <summary>
    /// 提现订单状态
    /// </summary>
    public enum DrawOrderStatus : byte
    {
        /// <summary>
        /// 待审核
        /// </summary>
        PendingAudit = 0,
        /// <summary>
        /// 处理中
        /// </summary>
        InTreatment = 1,
        /// <summary>
        /// 交易成功
        /// </summary>
        TradeSuccess = 2,
        /// <summary>
        /// 审核失败
        /// </summary>
        AuditFailure = 3,
        /// <summary>
        /// 表示空值，一般用于查询
        /// </summary>
        None = 4
    }
    /// <summary>
    /// 供应商提现订单表
    /// </summary>
    [Serializable]
    public sealed class MemberDrawOrder : NoIdentityModule
    {
        /// <summary>
        /// 主键，订单编号（时间加用户编号）
        /// </summary>
        [DataColumn(true, 64)]
        public string OrderId = null;
        /// <summary>
        ///用户id
        /// </summary>
        public long UserId = 0L;
        /// <summary>
        /// 银行名称
        /// </summary>
        [DataColumn(64)]
        public string BankName = null;
        /// <summary>
        /// 开户名
        /// </summary>
        [DataColumn(64)]
        public string AccountName = null;
        /// <summary>
        /// 开户网点
        /// </summary>
        [DataColumn(64)]
        public string BankZone = null;
        /// <summary>
        /// 银行卡号
        /// </summary>
        [DataColumn(64)]
        public string BankCard = null;
        /// <summary>
        /// 提现金额
        /// </summary>
        public Money DrawMoney = 0;
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime CreateTime = (DateTime)Types.GetDefaultValue(Templates.TType<DateTime>.Type);
        /// <summary>
        /// 订单状态，0、待审核1、处理中2、交易成功、-1、审核失败
        /// </summary>
        public DrawOrderStatus OrderState = DrawOrderStatus.PendingAudit;
        /// <summary>
        /// 审核失败理由
        /// </summary>
        [DataColumn(128)]
        public string RefusalReasons = null;
        /// <summary>
        /// 凭据图片，多张以"|"隔开
        /// </summary>
        [DataColumn(512)]
        public string CredentialImage = null;
        /// <summary>
        /// 转账成功后的交易单号
        /// </summary>
        [DataColumn(64)]
        public string TransactionNumber = null;
        /// <summary>
        /// 订单状态进入处理中时间
        /// </summary>
        public DateTime ProcessingDateTime = (DateTime)Types.GetDefaultValue(Templates.TType<DateTime>.Type);
        /// <summary>
        /// 订单状态进入交易成功时间
        /// </summary>
        public DateTime TradeSuccessDateTime = (DateTime)Types.GetDefaultValue(Templates.TType<DateTime>.Type);
        /// <summary>
        /// 订单状态进入审核失败时间
        /// </summary>
        public DateTime AuditFailureDateTime = (DateTime)Types.GetDefaultValue(Templates.TType<DateTime>.Type);

        public string GetOrderStateText()
        {
            switch (OrderState)
            {
                case DrawOrderStatus.AuditFailure: return "审核失败";
                case DrawOrderStatus.InTreatment: return "处理中";
                case DrawOrderStatus.PendingAudit: return "待审核";
                case DrawOrderStatus.TradeSuccess: return "交易成功";
                default: return "错误的状态";
            }
        }
        /// <summary>
        /// 是否是提现到余额
        /// </summary>
        /// <returns></returns>
        public bool IsDrawBalance()
        {
            return string.IsNullOrEmpty(AccountName)
                && string.IsNullOrEmpty(BankCard)
                && string.IsNullOrEmpty(BankName)
                && string.IsNullOrEmpty(BankZone);
        }
        /// <summary>
        /// 获取所有订单状态列表
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<DrawOrderStatus, string>> GetOrderStateList()
        {
            return new List<KeyValuePair<DrawOrderStatus, string>>
            {
                new KeyValuePair<DrawOrderStatus, string>(DrawOrderStatus.AuditFailure,"审核失败"),
                new KeyValuePair<DrawOrderStatus, string>(DrawOrderStatus.InTreatment,"处理中"),
                new KeyValuePair<DrawOrderStatus, string>(DrawOrderStatus.PendingAudit,"待审核"),
                new KeyValuePair<DrawOrderStatus, string>(DrawOrderStatus.TradeSuccess,"交易成功")
            };
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (DrawMoney <= 0 || UserId <= 0 || string.IsNullOrEmpty(OrderId))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {

            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(BankName))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            return DataStatus.Success;
        }

        public static SplitPageData<MemberDrawOrder> GetPagesByUserId(
            DataSource ds,
            long userid,
            int pageIndex,
            int size,
            DateTime stDate,
            DateTime endDate,
            DrawOrderStatus state,
            int show = 8)
        {
            long count;

            DbWhereQueue where = W("UserId", userid);
            if (DateTime.MinValue != stDate)
                where = where & W("CreateTime", stDate.ToString("yyyy-MM-dd HH:mm:ss"), DbWhereType.GreaterThanOrEqual);
            if (DateTime.MinValue != endDate)
            {
                endDate = stDate == endDate ? endDate.Add(new TimeSpan(23, 59, 59)) : endDate;
                where = where & W("CreateTime", endDate.ToString("yyyy-MM-dd HH:mm:ss"), DbWhereType.LessThanOrEqual);
            }
            if (state != DrawOrderStatus.None)
                where = where & W("OrderState", (byte)state);

            IList<MemberDrawOrder> list = Db<MemberDrawOrder>
                                         .Query(ds)
                                         .Select(S("CreateTime"), S("BankCard"), S("OrderId"), S("OrderState"), S("DrawMoney"))
                                         .Where(where)
                                         .OrderBy(D("CreateTime"))
                                         .ToList<MemberDrawOrder>(size, pageIndex, out count);

            return new SplitPageData<MemberDrawOrder>(pageIndex, size, list, count, show);
        }
        /// <summary>
        /// 获取待申请的
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Money GetPendingAuditMoney(DataSource ds,long userId)
        {
            Money PendingAuditMoney = Db<MemberDrawOrder>.Query(ds).Select(S_SUM("DrawMoney"))
                .Where(W("OrderState", DrawOrderStatus.PendingAudit) & W("UserId", userId))
                .First<MemberDrawOrder>().DrawMoney;
            return PendingAuditMoney;
        }
        /// <summary>
        /// 获取转账中的
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Money GetInTreatmentMoney(DataSource ds, long userId)
        {
            Money InTreatmentMoney = Db<MemberDrawOrder>.Query(ds).Select(S_SUM("DrawMoney"))
                .Where(W("OrderState", DrawOrderStatus.InTreatment) & W("UserId", userId))
                .First<MemberDrawOrder>().DrawMoney;
            return InTreatmentMoney;
        }
        /// <summary>
        /// 获取提现成功的
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Money GetDrawMoney(DataSource ds, long userId)
        {
            Money DrawMoney = Db<MemberDrawOrder>.Query(ds).Select(S_SUM("DrawMoney"))
                .Where(W("OrderState", DrawOrderStatus.TradeSuccess) & W("UserId", userId))
                .First<MemberDrawOrder>().DrawMoney;
            return DrawMoney;
        }

        public static SplitPageData<DataJoin<MemberDrawOrder, MemberBankInfo>> GetListByUserId(DataSource ds, long userid, int pageIndex, int size, DateTime stDate, DateTime endDate, DrawOrderStatus state, int show = 8)
        {
            long count;

            DbWhereQueue where = W<MemberDrawOrder>("UserId", userid);
            if (DateTime.MinValue != stDate)
                where = where & W<MemberDrawOrder>("CreateTime", stDate.ToString("yyyy-MM-dd HH:mm:ss"), DbWhereType.GreaterThanOrEqual);
            if (DateTime.MinValue != endDate)
            {
                endDate = stDate == endDate ? endDate.Add(new TimeSpan(23, 59, 59)) : endDate;
                where = where & W<MemberDrawOrder>("CreateTime", endDate.ToString("yyyy-MM-dd HH:mm:ss"), DbWhereType.LessThanOrEqual);
            }
            if (state != DrawOrderStatus.None)
                where = where & W("OrderState", (byte)state);

            IList<DataJoin<MemberDrawOrder, MemberBankInfo>> list = Db<MemberDrawOrder>
                                         .Query(ds)
                                         .Select(S<MemberDrawOrder>(), S<MemberBankInfo>("Icon"))
                                         .LeftJoin(O<MemberDrawOrder>("BankName"),O<MemberBankInfo>("BankName"))
                                         .Where(where)
                                         .OrderBy(D<MemberDrawOrder>("CreateTime"))
                                         .ToList<DataJoin<MemberDrawOrder, MemberBankInfo>>(size, pageIndex, out count);

            return new SplitPageData<DataJoin<MemberDrawOrder, MemberBankInfo>>(pageIndex, size, list, count, show);
        }

        public static MemberDrawOrder GetById(DataSource ds, long orderId)
        {
            return Db<MemberDrawOrder>
                    .Query(ds)
                    .Select()
                    .Where(new DbWhereQueue("OrderId", orderId))
                    .First<MemberDrawOrder>();
        }

        public static SplitPageData<DataJoin<MemberDrawOrder, Member>> GetPresentAuditList(DataSource ds, string query, int pageIndex, int size = 10)
        {
            long count;
            DbWhereQueue where = new DbWhereQueue();
            if (!string.IsNullOrEmpty(query))
            {
                where = W<Member>("Name", query, DbWhereType.Equal)
                  & W<Member>("Mobile", query, DbWhereType.Equal)
                  & W("Email", query, DbWhereType.Equal);
            }
            IList<DataJoin<MemberDrawOrder, Member>> list = Db<MemberDrawOrder>
                .Query(ds)
                .Select(S<MemberDrawOrder>("OrderId"),
                S<MemberDrawOrder>("AccountName"),
                S<MemberDrawOrder>("BankName"),
                S<MemberDrawOrder>("BankCard"),
                S<MemberDrawOrder>("BankZone"),
                S<MemberDrawOrder>("DrawMoney"),
                S<MemberDrawOrder>("OrderState"),
                S<MemberDrawOrder>("CreateTime"),
                S<MemberDrawOrder>("TransactionNumber"),
                S<MemberDrawOrder>("CredentialImage"),
                S<MemberDrawOrder>("RefusalReasons"),
                S<Member>("Name"),
                S<Member>("Mobile"))
                .LeftJoin(O<MemberDrawOrder>("UserId"), O<Member>("Id"))
                .Where(where)
                .OrderBy(A<MemberDrawOrder>("OrderState"), D<MemberDrawOrder>("CreateTime"))
                .ToList<DataJoin<MemberDrawOrder, Member>>(size, pageIndex, out count);

            return new SplitPageData<DataJoin<MemberDrawOrder, Member>>(pageIndex, size, list, (int)count);
        }
    }
}
