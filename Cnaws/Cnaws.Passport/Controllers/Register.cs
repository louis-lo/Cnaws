using Cnaws.Data;
using Cnaws.ExtensionMethods;
using Cnaws.Net;
using Cnaws.Web;
using Cnaws.Web.Configuration;
using Cnaws.Web.Controllers;
using System;
using System.Web.Configuration;
using M = Cnaws.Passport.Modules;
using S = Cnaws.Sms.Modules;
using V = Cnaws.Verification.Modules;

namespace Cnaws.Passport.Controllers
{
    public class Register : Sms.Controllers.Sms
    {
        public void Index()
        {
            string target = Request.QueryString["target"];
            if (string.IsNullOrEmpty(target))
            {
                if (Request.UrlReferrer != null)
                    target = Request.UrlReferrer.ToString();
            }
            this["Target"] = target;
            this["Config"] = PassportSection.GetSection();
            this["Sms"] = SMSCaptchaSection.GetSection();
            Render("register.html");
        }

        public void Reg()
        {
            string target = Request.QueryString["target"];
            if (string.IsNullOrEmpty(target))
            {
                if (Request.UrlReferrer != null)
                    target = Request.UrlReferrer.ToString();
            }
            this["Target"] = target;
            this["Config"] = PassportSection.GetSection();
            this["Sms"] = SMSCaptchaSection.GetSection();
            Render("reg.html");
        }

        [HttpPost]
        [HttpAjax]
        public void CheckName()
        {
            try
            {
                string name = Request.Form["Name"];
                if (!string.IsNullOrEmpty(name))
                    SetResult(M.Member.CheckName(DataSource, name));
                else
                    SetResult(false);
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [HttpPost]
        [HttpAjax]
        public void CheckEmail()
        {
            try
            {
                string name = Request.Form["Email"];
                if (!string.IsNullOrEmpty(name))
                    SetResult(M.Member.CheckEmail(DataSource, name));
                else
                    SetResult(false);
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [HttpPost]
        [HttpAjax]
        public void CheckMobile()
        {
            try
            {
                long mobile = long.Parse(Request.Form["Mobile"]);
                if (mobile.IsMobile())
                    SetResult(M.Member.CheckMobile(DataSource, mobile));
                else
                    SetResult(false);
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [HttpPost]
        [HttpAjax]
        public void SendSms(string name, int type)
        {
            SetResult(V.MobileHash.Sms(name, type, DataSource));
        }

        [HttpPost]
        [HttpAjax]
        public void Submit()
        {
            try
            {
                M.RegisterType type = (M.RegisterType)int.Parse(Request.Form["RegisterType"]);
                PassportSection section = PassportSection.GetSection();
                M.Member member = DbTable.Load<M.Member>(Request.Form);
                if (type == M.RegisterType.Mobile)
                {
                    if (section.VerifyMobile)
                    {
                        if (!V.MobileHash.Equals(DataSource, member.Mobile, V.MobileHash.Register, Request.Form["SmsCaptcha"]))
                        {
                            SetResult((int)M.LoginStatus.SmsCaptchaError);
                            return;
                        }
                        member.VerMob = true;
                    }
                }
                if (!IsWap)
                {
                    if (section.RegisterWithCaptcha)
                    {
                        if (!Captcha.CheckCaptcha(Request.Form["CaptchaName"], Request.Form["Captcha"]))
                        {
                            SetResult((int)M.LoginStatus.CaptchaError);
                            return;
                        }
                    }
                }
                string password = member.Password;
                if (member.ParentId == 0)
                {
                    bool convertResult = long.TryParse(Request.QueryString["ParentId"], out member.ParentId);
                    if (!convertResult)
                    {
                        member.ParentId = Utility.GetReference(this, DataSource);
                    }
                }
                member.Approved = section.DefaultApproved;
                member.CreationDate = DateTime.Now;
                DataStatus status = member.Insert(DataSource);
                if (status == DataStatus.Success)
                {
                    int errCount;
                    string name;
                    switch (type)
                    {
                        case M.RegisterType.Email: name = member.Email; break;
                        case M.RegisterType.Mobile: name = member.Mobile.ToString(); break;
                        default: name = member.Name; break;
                    }
                    M.LoginStatus state = M.Member.Login(DataSource, name, password, ClientIp, out errCount, out member);
                    if (state == M.LoginStatus.Success)
                        Web.PassportAuthentication.SetAuthCookie(true, false, member);
                    SetResult((int)state);
                }
                else
                {
                    SetResult((int)status);
                }
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }
    }
}
