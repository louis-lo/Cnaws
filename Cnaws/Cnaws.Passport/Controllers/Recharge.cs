using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Passport.Modules;
using Cnaws.Pay;
using Cnaws.Pay.Controllers;
using Cnaws.Pay.Modules;
using System.Text.RegularExpressions;

namespace Cnaws.Passport.Controllers
{
    public class Recharge : PaymentBase
    {
        private static readonly Regex MicroMessengerRegex = new Regex(@"MicroMessenger", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string PayProvider
        {
            get
            {
                if (IsWap)
                {
                    Match m = MicroMessengerRegex.Match(Request.UserAgent);
                    if (m.Success)
                        return "wxpay";
                    return "alipaymobile";
                }
                return "alipaydirect";
            }
        }
        public string PayName
        {
            get { return LoadProvider(PayProvider).Name; }
        }

        protected override void OnIndex()
        {
            this["Member"] = M.MemberInfo.GetByRecharge(DataSource, User.Identity.Id);
            Render("recharge.html");
        }

        protected override bool CheckProvider(PayProvider provider)
        {
            return provider.IsOnlinePay;
        }
        protected override IPayOrder GetPayOrder(string provider)
        {
            string openId = null;
            M.OAuth2Member member = M.OAuth2Member.GetByUserPay(DataSource, provider, User.Identity.Id);
            if (member != null)
                openId = member.UserId;
            return PayRecord.Create(DataSource, User.Identity.Id, openId, "充值", provider, Money.Parse(Request.Form["Money"]), 0, string.Empty);
        }

        protected override bool OnModifyMoney(PayProvider provider, PaymentType payment, long user, Money money, string trade, string title, int type, string targetId)
        {
            try
            {
                if (payment == PaymentType.Pay)
                    return M.MemberInfo.ModifyMoney(DataSource, user, money, title, type, trade) == DataStatus.Success;
            }
            catch (Exception) { }
            return false;
        }
    }
}
