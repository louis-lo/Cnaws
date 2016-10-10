using System;
using System.Web;
using System.Collections;
using Cnaws.Web;
using Cnaws.Templates;
using Cnaws.Passport.OAuth2;
using M = Cnaws.Passport.Modules;
using Cnaws.Web.Configuration;
using Cnaws.Data;
using S = Cnaws.Sms.Modules;
using V = Cnaws.Verification.Modules;

namespace Cnaws.Passport.Controllers
{
    public class OAuth2 : DataController
    {
        private OAuth2Provider GetProvider(string type, bool referer = false)
        {
            type = type.ToLower();
            Uri uri = Request.Url;
            string redirect = string.Concat(uri.Scheme, "://", uri.DnsSafeHost, uri.Port != 80 ? string.Concat(":", uri.Port.ToString()) : string.Empty, GetUrl(string.Concat("/oauth2/cb/", type)));
            if (referer)
            {
                string target = Request.QueryString["target"];
                if (string.IsNullOrEmpty(target))
                {
                    if (Request.UrlReferrer != null)
                        target = Request.UrlReferrer.ToString();
                }
                if (!string.IsNullOrEmpty(target))
                    redirect = string.Concat(redirect, "?target=", HttpUtility.UrlEncode(target));
            }
            M.OAuth2 provider = M.OAuth2.GetById(DataSource, type);
            if (provider != null)
            {
                OAuth2ProviderOptions options = new OAuth2ProviderOptions()
                {
                    ClientId = provider.Key,
                    ClientSecret = provider.Secret,
                    RedirectUri = redirect
                };
                return OAuth2Provider.Create(options, type);
            }
            return null;
        }

        public void Login(string type)
        {
            GetProvider(type, true).Authorize();
        }
        public void Cb(string type)
        {
            string target = Request.QueryString["target"];
            if (string.IsNullOrEmpty(target))
                target = GetUrl("/");
            OAuth2Provider provider = GetProvider(type);
            if (provider != null)
            {
                OAuth2TokenAccess token = provider.Access() as OAuth2TokenAccess;
                M.OAuth2Member user = provider.GetUserInfo(token);
                if (!string.IsNullOrEmpty(user.UserId))
                {
                    M.Member member;
                    M.LoginStatus status = M.OAuth2Member.Login(DataSource, type.ToLower(), user.UserId, ClientIp, out member);
                    if (status == M.LoginStatus.Success)
                    {
                        PassportAuthentication.SetAuthCookie(true, false, member);
                        OnLogined(member.Id);
                        Refresh(target);
                    }
                    else
                    {
                        if (status == M.LoginStatus.NeedBind)
                        {
                            this["Oauth2Type"] = user.Type;
                            this["Oauth2UserId"] = user.UserId;
                            this["Target"] = target;
                            this["Sms"] = SMSCaptchaSection.GetSection();
                            Render("oauth2.html");
                        }
                        else
                        {
                            Redirect(GetUrl("/login"));
                        }
                    }
                }
                else
                {
                    Redirect(GetUrl("/login"));
                }
            }
            else
            {
                Redirect(GetUrl("/login"));
            }
        }

        [HttpPost]
        [HttpAjax]
        public void Bind()
        {
            try
            {
                //string target = Request.Form["Target"];
                //if (string.IsNullOrEmpty(target))
                //    target = GetUrl("/");
                string type = Request.Form["Oauth2Type"];
                string userId = Request.Form["Oauth2UserId"];
                M.RegisterType rt = (M.RegisterType)int.Parse(Request.Form["RegisterType"]);
                M.Member member = DbTable.Load<M.Member>(Request.Form);
                if (rt == M.RegisterType.Mobile)
                {
                    if (!V.MobileHash.Equals(DataSource, member.Mobile, V.MobileHash.Register, Request.Form["Captcha"]))
                    {
                        SetResult((int)M.LoginStatus.CaptchaError);
                        return;
                    }
                    member.VerMob = true;
                }
                string password = member.Password;
                member.ParentId = Utility.GetReference(this, DataSource);
                member.Approved = true;
                member.CreationDate = DateTime.Now;
                DataStatus status = M.OAuth2Member.Register(DataSource, type, userId, member, rt);
                if (status == DataStatus.Success)
                {
                    M.LoginStatus state = M.OAuth2Member.Login(DataSource, type, userId, ClientIp, out member);
                    if (state == M.LoginStatus.Success)
                    {
                        PassportAuthentication.SetAuthCookie(true, false, member);
                        OnLogined(member.Id);
                        SetResult(true);
                    }
                    else
                    {
                        SetResult((int)state);
                    }
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

        protected virtual void OnLogined(long userId)
        {

        }
    }
}
