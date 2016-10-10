using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using Cnaws.Web.Configuration;
using System.Text;
using Cnaws.Data.Query;
using System.Collections.Specialized;
using Cnaws.Web.Controllers;
using System.Web;
using Cnaws.Sms.Modules;
using Cnaws.Sms;
using M = Cnaws.Sms.Modules;

namespace Cnaws.Verification.Modules
{
    [Serializable]
    public sealed class MobileHash : NoIdentityModule
    {
        public const int Register = 0;
        public const int Password = 1;

        [DataColumn(true)]
        public long Mobile = 0;
        [DataColumn(true)]
        public int Type = 0;
        [DataColumn(36)]
        public string Hash = null;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        private static string RndHash()
        {
            SMSCaptchaSection section = SMSCaptchaSection.GetSection();
            StringBuilder sb = new StringBuilder(section.DefaultCount);
            Random rnd = new Random();
            for (int i = 0; i < section.DefaultCount; ++i)
                sb.Append(section.Chars[rnd.Next(0, section.DefaultCount)]);
            return sb.ToString();
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (!Mobile.IsMobile())
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (!Mobile.IsMobile())
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }
        
        public static MobileHash Create(DataSource ds, long mobile, int type, int timespan)
        {
            MobileHash hash = Db<MobileHash>.Query(ds)
                .Select()
                .Where(W("Type", type) & W("Mobile", mobile))
                .First<MobileHash>();
            if (hash != null)
            {
                if (hash.CreationDate.AddSeconds(timespan) > DateTime.Now)
                    return null;
                hash.Hash = RndHash();
                hash.CreationDate = DateTime.Now;
                if (hash.Update(ds) == DataStatus.Success)
                    return hash;
            }
            else
            {
                hash = new MobileHash() { Mobile = mobile, Type = type, Hash = RndHash(), CreationDate = DateTime.Now };
                if (hash.Insert(ds) == DataStatus.Success)
                    return hash;
            }
            return null;
        }

        public static bool Equals(DataSource ds, long mobile, int type, string hash)
        {
            if (!mobile.IsMobile() || string.IsNullOrEmpty(hash))
                return false;
            MobileHash mh = ExecuteSingleRow<MobileHash>(ds, P("Type", type) & P("Mobile", mobile));
            if (mh == null)
                return false;
            if (!string.Equals(mh.Hash, hash, StringComparison.OrdinalIgnoreCase))
                return false;
            SMSCaptchaSection section = SMSCaptchaSection.GetSection();
            if (section.Expiration > 0 && mh.CreationDate.AddSeconds(section.Expiration) < DateTime.Now)
                return false;
            mh.Hash = string.Empty;
            mh.CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
            mh.Update(ds);
            return true;
        }
        /// <summary>
        /// 仅验证不操作验证数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="mobile"></param>
        /// <param name="type"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool CheckAndNotOperation(DataSource ds, long mobile, int type, string hash)
        {
            if (!mobile.IsMobile() || string.IsNullOrEmpty(hash))
                return false;
            MobileHash mh = ExecuteSingleRow<MobileHash>(ds, P("Type", type) & P("Mobile", mobile));
            if (mh == null)
                return false;
            if (!string.Equals(mh.Hash, hash, StringComparison.OrdinalIgnoreCase))
                return false;
            SMSCaptchaSection section = SMSCaptchaSection.GetSection();
            if (section.Expiration > 0 && mh.CreationDate.AddSeconds(section.Expiration) < DateTime.Now)
                return false;
            return true;
        }

        public static bool Sms(string name, int type, DataSource ds)
        {
            try
            {
                PassportSection section = PassportSection.GetSection();
                if (!section.VerifyMobile)
                    throw new Exception();

                HttpRequest Request = HttpContext.Current.Request;
                string captcha = Request.Form["Captcha"];
                if (!string.IsNullOrEmpty(captcha))
                {
                    if (!Captcha.CheckCaptcha(Request.Form["CaptchaName"], captcha))
                        throw new Exception();
                }

                long mobile = long.Parse(Request.Form["Mobile"]);
                int timespan = SMSCaptchaSection.GetSection().TimeSpan;
                MobileHash hash = MobileHash.Create(ds, mobile, type, timespan);
                if (hash == null)
                    throw new Exception();

                string md5 = string.Concat(Request.UserHostAddress, "\r\n", Request.UserAgent).MD5();
                StringHash sh = StringHash.Create(ds, md5, StringHash.SmsHash, timespan);
                if (sh == null)
                    throw new Exception();

                SmsTemplate temp = SmsTemplate.GetByName(ds, SmsTemplate.Register);
                if (temp.Type == SmsTemplateType.Template)
                    SendTemplateImpl(name, mobile, temp.Content, ds, hash.Hash);
                else
                    SendImpl(name, mobile, temp.Content, ds, hash.Hash);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SendImpl(string name, long mobile, string body, DataSource ds, params string[] args)
        {
            SmsProvider provider = SmsProvider.Create(name);
            M.Sms sms = M.Sms.GetById(ds, provider.Key);
            if (!sms.Enabled)
                throw new Exception();
            provider.Account = sms.Account;
            provider.Token = sms.Token;
            provider.AppId = sms.AppId;
            provider.Log = new SmsSqlLog(ds);
            provider.Send(mobile, body, args);
        }

        private static void SendTemplateImpl(string name, long mobile, string temp, DataSource ds, params string[] args)
        {
            SmsProvider provider = SmsProvider.Create(name);
            M.Sms sms = M.Sms.GetById(ds, provider.Key);
            if (!sms.Enabled)
                throw new Exception();
            provider.Account = sms.Account;
            provider.Token = sms.Token;
            provider.AppId = sms.AppId;
            provider.Log = new Cnaws.Sms.SmsSqlLog(ds);
            provider.SendTemplate(mobile, temp, args);
        }
    }
}
