using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Web.Configuration;
using M = Cnaws.Passport.Modules;
using S = Cnaws.Sms.Modules;
using V = Cnaws.Verification.Modules;
using Cnaws.Web.Controllers;
using Cnaws.ExtensionMethods;

namespace Cnaws.Passport.Controllers
{
    public sealed class Password : Sms.Controllers.Sms
    {
        public void Index()
        {
            this["Sms"] = SMSCaptchaSection.GetSection();
            Render("password.html");
        }

        [HttpPost]
        [HttpAjax]
        public void CheckUser()
        {
            try
            {
                if (!Captcha.CheckCaptcha(Request.Form["CaptchaName"], Request.Form["Captcha"]))
                {
                    SetResult((int)M.LoginStatus.CaptchaError);
                    return;
                }
                M.Member member = M.Member.Get(DataSource, Request.Form["UserName"]);
                if (member == null)
                {
                    SetResult((int)M.LoginStatus.NotFound);
                    return;
                }
                SetResult(true, member.Mobile);
            }
            catch (Exception ex)
            {
                SetResult(false, ex.Message);
            }
        }

        [HttpPost]
        [HttpAjax]
        public void Submit()
        {
            try
            {
                M.Member member = DbTable.Load<M.Member>(Request.Form);
                if (!V.MobileHash.Equals(DataSource, member.Mobile, V.MobileHash.Password, Request.Form["Captcha"]))
                {
                    SetResult((int)M.LoginStatus.CaptchaError);
                    return;
                }
                M.Member temp = M.Member.Get(DataSource, member.Mobile.ToString());
                if (temp == null)
                {
                    SetResult((int)M.LoginStatus.NotFound);
                    return;
                }
                member.Id = temp.Id;
                SetResult(member.Update(DataSource, ColumnMode.Include, "Password") == DataStatus.Success);
            }
            catch (Exception ex)
            {
                SetResult(false, ex.Message);
            }
        }

        [HttpPost]
        [HttpAjax]
        public void SendSms(string name)
        {
            try
            {
                PassportSection section = PassportSection.GetSection();
                if (!section.VerifyMobile)
                    throw new Exception();

                long mobile = long.Parse(Request.Form["Mobile"]);
                int timespan = SMSCaptchaSection.GetSection().TimeSpan;
                V.MobileHash hash = V.MobileHash.Create(DataSource, mobile, V.MobileHash.Password, timespan);
                if (hash == null)
                    throw new Exception();

                string md5 = string.Concat(ClientIp, "\r\n", Request.UserAgent).MD5();
                V.StringHash sh = V.StringHash.Create(DataSource, md5, V.StringHash.SmsHash, timespan);
                if (sh == null)
                    throw new Exception();

                S.SmsTemplate temp = S.SmsTemplate.GetByName(DataSource, S.SmsTemplate.Register);
                if (temp.Type == S.SmsTemplateType.Template)
                    SendTemplateImpl(name, mobile, temp.Content, hash.Hash);
                else
                    SendImpl(name, mobile, temp.Content, hash.Hash);
                SetResult(true);
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }
    }
}
