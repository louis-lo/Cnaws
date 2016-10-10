using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Passport.Modules;
using Cnaws.ExtensionMethods;
using Cnaws.Web.Configuration;
using V = Cnaws.Verification.Modules;
using Cnaws.Data.Query;
using System.Text.RegularExpressions;

namespace Cnaws.Passport.Controllers
{
    public class Security : PassportController
    {
        [Authorize(true)]
        public void Index(string type = null, int step = 1)
        {
            this["Member"] = M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id);
            Render("security.html");
        }

        [HttpAjax]
        [HttpPost]
        [Authorize]
        public void Submit(string type, int step)
        {
            try
            {
                switch (type.ToLower())
                {
                    case "password":
                        {
                            switch (step)
                            {
                                case 1:
                                    {
                                        string p1 = Request.Form["Password"];
                                        string p2 = M.Member.GetPasswordById(DataSource, User.Identity.Id);
                                        if (!string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(p2))
                                        {
                                            p1 = p1.MD5();
                                            if (p2.Equals(p1))
                                            {
                                                Response.Cookies["CNAWS.PASSPORT.OLDPASSWORD"].Value = p1;
                                                SetResult(true);
                                            }
                                            else
                                            {
                                                SetResult(false);
                                            }
                                        }
                                        else
                                        {
                                            SetResult(false);
                                        }
                                    }
                                    break;
                                case 2:
                                    {
                                        System.Web.HttpCookie cookie = Request.Cookies["CNAWS.PASSPORT.OLDPASSWORD"];
                                        if (cookie != null)
                                        {
                                            string password = M.Member.GetPasswordById(DataSource, User.Identity.Id);
                                            if (string.Equals(password, cookie.Value))
                                                SetResult((new M.Member() { Id = User.Identity.Id, Password = Request.Form["Password"] }).Update(DataSource, ColumnMode.Include, "Password") == DataStatus.Success);
                                            else
                                                SetResult((int)M.LoginStatus.PasswordError);
                                        }
                                        else
                                        {
                                            SetResult((int)M.LoginStatus.PasswordError);
                                        }
                                    }
                                    break;
                                case 3:
                                    {
                                        string old = Request.Form["OldPassword"];
                                        string pwd = Request.Form["NewPassword"];
                                        string opwd = M.Member.GetPasswordById(DataSource, User.Identity.Id);
                                        if (!string.IsNullOrEmpty(old) && !string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(opwd))
                                        {
                                            old = old.MD5();
                                            if (opwd.Equals(old))
                                            {
                                                SetResult((new M.Member() { Id = User.Identity.Id, Password = pwd }).Update(DataSource, ColumnMode.Include, "Password") == DataStatus.Success);
                                            }
                                            else
                                            {
                                                SetResult(-1002);
                                            }
                                        }
                                        else
                                        {
                                            SetResult(false);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "email":
                        {
                            switch (step)
                            {
                                case 1:
                                    {

                                    }
                                    break;
                                case 2:
                                    {

                                    }
                                    break;
                                case 3:
                                    {

                                    }
                                    break;
                            }
                        }
                        break;
                    case "phone":
                        {
                            switch (step)
                            {
                                case 1:
                                    {
                                        long mobile;
                                        if (long.TryParse(Request.Form["Mobile"], out mobile))
                                        {
                                            //判断验证码是否正确
                                            if (!V.MobileHash.Equals(DataSource, mobile, V.MobileHash.Password, Request.Form["Code"]))
                                            {
                                                SetResult(-1002);
                                            }
                                            else
                                            { 
                                                M.MemberInfo memberInfo = M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id);
                                                //判断是否已绑定过手机号码，绑定过验证支付密码是否正确
                                                if (memberInfo.VerMob)
                                                {
                                                    if (string.Equals(Request.Form["PayPass"].MD5(), memberInfo.PayPassword))
                                                    {
                                                        if (Db<M.MemberInfo>.Query(DataSource).Update().Set("Mobile", mobile).Where(new DbWhereQueue("Id", User.Identity.Id)).Execute() > 0)
                                                        {
                                                            SetResult(DataStatus.Success);
                                                        }
                                                        else
                                                        {
                                                            SetResult(DataStatus.Failed);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SetResult(-1003);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Db<M.MemberInfo>.Query(DataSource).Update().Set("VerMob", true).Set("Mobile", mobile).Where(new DbWhereQueue("Id", User.Identity.Id)).Execute() > 0)
                                                    {
                                                        SetResult(DataStatus.Success);
                                                    }
                                                    else
                                                    {
                                                        SetResult(DataStatus.Failed);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SetResult(DataStatus.Failed);
                                        }
                                    }
                                    break;
                                case 2:
                                    {

                                    }
                                    break;
                                case 3:
                                    {

                                    }
                                    break;
                            }
                        }
                        break;
                    case "paypassword":
                        {
                            switch (step)
                            {
                                case 1:
                                    {
                                        string p1 = Request.Form["PayPassword"];
                                        string p2 = M.MemberInfo.GetPayPasswordById(DataSource, User.Identity.Id);
                                        if (!string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(p2))
                                        {
                                            p1 = p1.MD5();
                                            if (p2.Equals(p1))
                                            {
                                                Response.Cookies["CNAWS.PASSPORT.OLDPAYPASSWORD"].Value = p1;
                                                SetResult(true);
                                            }
                                            else
                                            {
                                                SetResult(false);
                                            }
                                        }
                                        else
                                        {
                                            SetResult(false);
                                        }
                                    }
                                    break;
                                case 2:
                                    {
                                        if (!string.IsNullOrEmpty(M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id).PayPassword))
                                        {
                                            System.Web.HttpCookie cookie = Request.Cookies["CNAWS.PASSPORT.OLDPAYPASSWORD"];
                                            if (cookie != null)
                                            {
                                                string password = M.MemberInfo.GetPayPasswordById(DataSource, User.Identity.Id);
                                                if (string.Equals(password, cookie.Value))
                                                    SetResult((new M.MemberInfo() { Id = User.Identity.Id, PayPassword = Request.Form["PayPassword"] }).Update(DataSource, ColumnMode.Include, "PayPassword") == DataStatus.Success);
                                                else
                                                    SetResult((int)M.LoginStatus.PasswordError);
                                            }
                                            else
                                            {
                                                SetResult((int)M.LoginStatus.PasswordError);
                                            }
                                        }
                                        else
                                        {
                                            SetResult((new M.MemberInfo() { Id = User.Identity.Id, PayPassword = Request.Form["PayPassword"] }).Update(DataSource, ColumnMode.Include, "PayPassword") == DataStatus.Success);
                                        }

                                    }
                                    break;
                                case 3:
                                    {
                                        string old = Request.Form["OldPayPassword"];
                                        string pwd = Request.Form["NewPayPassword"];
                                        string opwd = M.MemberInfo.GetPayPasswordById(DataSource, User.Identity.Id);
                                        bool empty = string.IsNullOrEmpty(M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id).PayPassword);
                                        if ((empty || !string.IsNullOrEmpty(old)) && !string.IsNullOrEmpty(pwd) && (empty || !string.IsNullOrEmpty(opwd)))
                                        {
                                            if (empty || opwd.Equals(old.MD5()))
                                            {
                                                SetResult((new M.MemberInfo() { Id = User.Identity.Id, PayPassword = pwd }).Update(DataSource, ColumnMode.Include, "PayPassword") == DataStatus.Success);
                                            }
                                            else
                                            {
                                                SetResult(false);
                                            }
                                        }
                                        else
                                        {
                                            SetResult(false);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [Authorize(true)]
        public void Default(string type, int step = 1)
        {
            this["Member"] = M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id);
            Render(string.Concat(type, step, ".html"));
        }

        [Authorize(true)]
        public void UpdatePhone()
        {
            this["Sms"] = SMSCaptchaSection.GetSection();
            this["Member"] = M.MemberInfo.GetBySecurity(DataSource, User.Identity.Id);
            Render("update_phone.html");
        }

        [Authorize(true)]
        public void setuname()
        {
            this["Member"] = M.MemberInfo.GetByModify(DataSource, User.Identity.Id);
            Render("setuname.html");
        }
        [Authorize]
        public void SubmitSetName()
        {
            Regex regex = new Regex(@"^[\u4e00-\u9fa50-9a-zA-Z_\-]{4,20}$");
            if (regex.IsMatch(Request.Form["NickName"]))
            {
                if (Db<M.Member>.Query(DataSource).Update().Set("NickName", Request.Form["NickName"]).Where(new DbWhereQueue("Id", User.Identity.Id)).Execute() > 0)
                {
                    SetResult(DataStatus.Success);
                }
                else
                {
                    SetResult(DataStatus.Failed);
                }
            }
            else
            {
                SetResult(-1002);
            }
        }
      
    }
}
