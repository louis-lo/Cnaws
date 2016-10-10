using Cnaws.Data;
using Cnaws.Passport.OAuth2;
using Cnaws.Web;
using Cnaws.Web.Configuration;
using Cnaws.Web.Controllers;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using M = Cnaws.Passport.Modules;

namespace Cnaws.Passport.Controllers
{
    public class Login : DataController
    {
        private static readonly Regex MicroMessengerRegex = new Regex(@"MicroMessenger", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void Index()
        {
            string target = Request.QueryString["target"];
            if (string.IsNullOrEmpty(target))
            {
                if (Request.UrlReferrer != null)
                {
                    if (!Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1].EndsWith("logout.html", StringComparison.InvariantCultureIgnoreCase))
                        target = Request.UrlReferrer.ToString();
                }
            }
            if (string.IsNullOrEmpty(target))
                target = GetUrl("ucenter");
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated && !User.Identity.IsAdmin)
            {
                Redirect(target);
            }
            else
            {
                if (IsWap)
                {
                    Match m = MicroMessengerRegex.Match(Request.UserAgent);
                    if (m.Success)
                    {
                        M.OAuth2 oa = M.OAuth2.GetById(DataSource, "weixin");
                        if (oa != null)
                        {
                            if (oa.Enabled)
                            {
                                Redirect(string.Concat(GetUrl("/oauth2/login/weixin"), "?target=", HttpUtility.UrlEncode(target)));
                                return;
                            }
                        }
                    }
                }
                HttpCookie Uname = Request.Cookies["UserName"];
                this["UserName"] = Uname != null ? Uname.Values["UName"] : "";
                this["Target"] = target;
                this["Config"] = PassportSection.GetSection();
                Render("login.html");
            }
        }
        public void Submit()
        {
            if (IsAjax)
            {
                if (IsPost)
                {
                    if (!IsWap)
                    {
                        if (PassportSection.GetSection().LoginWithCaptcha)
                        {
                            if (!Captcha.CheckCaptcha("login", Request.Form["Captcha"]))
                            {
                                SetResult((int)M.LoginStatus.CaptchaError);
                                return;
                            }
                        }
                    }
                    int errCount;
                    M.Member member;
                    string name = Request.Form["UserName"];
                    string pwd = Request.Form["Password"];
                    M.LoginStatus status = M.Member.Login(DataSource, name, pwd, ClientIp, out errCount, out member);
                    if (status == M.LoginStatus.Success)
                    {
                        Web.PassportAuthentication.SetAuthCookie(true, false, member);
                        OnLogined(member.Id);

                        HttpCookie loginCookie = new HttpCookie("UserName");
                        string check = Request.Form["remember"];
                        if (check == "true")
                        {
                            loginCookie.Values.Add("UName", name);
                            loginCookie.Expires = DateTime.Now.AddYears(1);
                            Response.SetCookie(loginCookie);
                        }
                        else
                        {
                            loginCookie.Values.Add("UName", "");
                            loginCookie.Expires = DateTime.Now.AddYears(1);
                            Response.SetCookie(loginCookie);
                        }
                    }
                    SetResult((int)status, errCount);
                }
                else
                {
                    NotFound();
                }
            }
            else
            {
                NotFound();
            }
        }
        protected virtual void OnLogined(long userId)
        {

        }
    }
}
