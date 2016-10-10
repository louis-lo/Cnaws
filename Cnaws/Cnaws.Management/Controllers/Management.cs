using Cnaws.Area;
using Cnaws.Data;
using Cnaws.Json;
using Cnaws.Templates;
using Cnaws.Web;
using Cnaws.Web.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using M = Cnaws.Management.Modules;
//using VM = Cnaws.Web.Modules;

namespace Cnaws.Management.Controllers
{
    public delegate void DataActionHandler(DataSource ds);
    public delegate object DataGetPageHandler(DataSource ds, int index, int size, int show);

    public sealed class Management : ResourceDataController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 5);

        private sealed class DataTableEx : IDbReader
        {
            private List<string> Keys;
            private List<List<object>> Values;

            public DataTableEx()
            {
                Keys = null;
                Values = null;
            }

            public bool IsEmpty
            {
                get { return Keys == null; }
            }

            void IDbReader.ReadRow(DbDataReader reader)
            {
                if (Keys == null)
                {
                    Keys = new List<string>();
                    for (int i = 0; i < reader.FieldCount; ++i)
                        Keys.Add(reader.GetName(i));
                }
                if (Values == null)
                    Values = new List<List<object>>();
                List<object> list = new List<object>();
                for (int i = 0; i < reader.FieldCount; ++i)
                    list.Add(reader.GetValue(i));
                Values.Add(list);
            }

            public override string ToString()
            {
                return JsonValue.Serialize<DataTableEx>(this);
            }
        }

        public override bool IsWap
        {
            get { return false; }
        }

        protected override Version Version
        {
            get { return VERSION; }
        }
        protected override string Namespace
        {
            get { return "Cnaws.Management"; }
        }

        public override string DataProvider
        {
            get { return Web.PassportAuthentication.DataProvider; }
        }

        protected override string GetVirtualUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Concat(Application.Settings.RootUrl, Application.Settings.Management);
            return GetUrl(string.Concat("/", Application.Settings.Management, "/", path));
        }
        protected override string GetVirtualRes(string path)
        {
            if (path.StartsWith("~/"))
                return base.GetVirtualRes(path.Substring(2));
            return GetRes(string.Concat("/", Application.Settings.Management, "/", path));
        }

        protected internal override void NotFound()
        {
            string m = Application.Action.Substring(Application.Action.IndexOf('.') + 1);
            if (IsHtml())
                Response.Write(string.Concat("<div class=\"rightmsg\">未安装\"", m, "\"模块</div>"));
            else
                SetResult(-404, m);
        }
        protected internal override void Unauthorized(bool redirect = false)
        {
            if (IsHtml())
                Response.Write("<div class=\"rightmsg\">权限不足</div>");
            else
                SetResult(-401);
        }

        internal void WriteLog(string msg)
        {
            M.LogOperation.Insert(DataSource, User.Identity.Name, Application.Action, msg);
        }

        private int GetInt32Value(string s, int d)
        {
            if (!string.IsNullOrEmpty(s))
            {
                int v;
                if (int.TryParse(s, out v))
                    return v;
            }
            return d;
        }
        private bool GetBooleanValue(string s)
        {
            if (!string.IsNullOrEmpty(s))
                return "on".EndsWith(s);
            return false;
        }

        public void Static(string name, Arguments args)
        {
            RenderResource(name, args, false);
        }

        public void Index()
        {
            if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
            {
                this["Role"] = M.AdminRole.GetById(DataSource, (int)User.Identity.RoleId);
                RenderTemplate("index.html");
            }
            else
            {
                Redirect(GetUrl("login"));
            }
        }
        public void Login()
        {
            if (IsAjax)
            {
                if (IsPost)
                {
                    if (Cnaws.Web.Controllers.Captcha.CheckCaptcha("syslogin", Request.Form["Captcha"]))
                    {
                        int count;
                        M.Admin admin = M.Admin.Login(DataSource, Request.Form["Name"], Request.Form["Password"], ClientIp, out count);
                        if (admin != null)
                        {
                            if (admin.Locked)
                            {
                                SetResult(-3);
                            }
                            else
                            {

                                Web.PassportAuthentication.SetAuthCookie<ManagementPassport>(true, true, admin, Context);
                                WriteLog("login");
                                SetResult(true);
                            }
                        }
                        else
                        {
                            SetResult(-2, count);
                        }
                    }
                    else
                    {
                        SetResult(-1);
                    }
                }
                else
                {
                    NotFound();
                }
            }
            else
            {
                if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
                    Redirect(GetUrl(""));
                else
                    RenderTemplate("login.html");
            }
        }
        public void Logout()
        {
            if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
            {
                WriteLog("logout");
                Cnaws.Web.PassportAuthentication.SignOut();
            }
            Response.Write(string.Concat("<script type=\"text/javascript\">window.top.location.href=\"", GetUrl("login"), "\";</script>"));
        }
        public void Password()
        {
            if (CheckAjax())
            {
                if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
                {
                    if (IsPost)
                    {
                        SetResult(M.Admin.ChangePassword(DataSource, (int)User.Identity.AdminId, Request.Form["ConPass"], Request.Form["OldPass"]), () =>
                        {
                            WriteLog("change password");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
                else
                {
                    Unauthorized();
                }
            }
        }
        public void Welcome()
        {
            if (CheckRight())
            {
                if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
                {
                    if (CheckPost("welcome", () =>
                     {
                         StringBuilder sb = new StringBuilder();
                         List<ModuleInfo> list = ModuleInfo.GetAllList(Context);
                         foreach (ModuleInfo info in list)
                         {
                             sb.Append("<li><i class=\"dll\">");
                             sb.Append(info.Name);
                             sb.Append("</i><i class=\"ii\">&nbsp;v</i><i class=\"ver\">");
                             sb.Append(info.Version);
                             sb.Append("</i><i class=\"up\"></i></li>");
                         }
                         this["Version"] = sb.ToString();
                         string loginArea = User.Identity.LastIp;
                         using (IPArea area = new IPArea())
                         {
                             IPLocation ip = area.Search(loginArea);
                             if (ip != null)
                                 loginArea = ip.ToString();
                         }
                         this["LoginArea"] = loginArea;
                     }))
                    {
                        NotFound();
                    }
                }
                else
                {
                    Unauthorized();
                }
            }
        }
        public void Upgrade()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    try
                    {
                        byte[] data = Encoding.UTF8.GetBytes(JsonValue.Serialize(ModuleInfo.GetAllList(Context)));
                        HttpWebRequest request = WebRequest.CreateHttp("http://upgrade.cnaws.com/upgrade/check.html");
                        request.AllowAutoRedirect = true;
                        request.Method = "POST";
                        request.ContentType = "application/x-javascript";
                        using (Stream input = request.GetRequestStream())
                            input.Write(data, 0, data.Length);
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            using (Stream output = response.GetResponseStream())
                            {
                                using (StreamReader reader = new StreamReader(output, Encoding.UTF8))
                                    SetResult(JsonValue.Deserialize<Dictionary<string, string>>(reader.ReadToEnd()));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        SetResult(false);
                    }
                }
            }
        }
        public void System(string type)
        {
            if (CheckRight())
            {
                if (CheckPost(string.Concat("system_", type), new Action(() =>
                {
                    switch (type)
                    {
                        case "config":
                            this["Config"] = new
                            {
                                Theme = Application.Settings.Theme,
                                UrlMode = Application.Settings.UrlMode.ToString(),
                                Management = Application.Settings.Management,
                                Themes = Application.Settings.GetThemes(),
                                WapDomain = Application.Settings.WapDomain,
                                SubDomain = Application.Settings.SubDomain,
                                DataProvider = Application.Settings.DataProvider,
                                CacheMode = Application.Settings.CacheMode,
                                CacheProvider = Application.Settings.CacheProvider,
                                ResourcesUrl = Application.Settings.ResourcesUrl,
                                PassportUrl = Application.Settings.PassportUrl
                            };
                            this["Conns"] = WebConfigurationManager.ConnectionStrings;
                            break;
                        case "passport":
                            this["Config"] = PassportSection.GetSection();
                            this["Conns"] = WebConfigurationManager.ConnectionStrings;
                            break;
                        case "captcha":
                            this["Config"] = CaptchaSection.GetSection();
                            break;
                        case "smscaptcha":
                            this["Config"] = SMSCaptchaSection.GetSection();
                            break;
                        case "filesystem":
                            this["Config"] = FileSystemSection.GetSection();
                            break;
                        case "email":
                            this["Config"] = (SmtpSection)WebConfigurationManager.GetSection("system.net/mailSettings/smtp");
                            break;
                        case "friendlink":
                            this["Config"] = FriendLinkSection.GetSection();
                            break;
                        case "robots":
                            {
                                string content = string.Empty;
                                try
                                {
                                    string file = Server.MapPath("~/robots.txt");
                                    content = Encoding.UTF8.GetString(File.ReadAllBytes(file));
                                }
                                catch (Exception) { }
                                this["Robots"] = content;
                            }
                            break;
                        case "sitemap":
                            break;
                    }
                })))
                {
                    switch (type)
                    {
                        case "site":
                            {
                                int i = 0;
                                StringBuilder sb = new StringBuilder();
                                foreach (string key in Request.Form.Keys)
                                {
                                    if (i++ > 0)
                                        sb.Append(',');
                                    sb.Append(string.Concat("\"", key, "\"=\"", HttpUtility.HtmlEncode(Request.Form[key]), "\""));
                                }
                                try
                                {
                                    File.WriteAllText(Server.MapPath(string.Concat(Utility.ConfigDir, "site.config")), string.Concat("$set(Site=array(", sb.ToString(), "))"));
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "config":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    SiteSection ss = SiteSection.GetSection(config);
                                    ss.Theme = Request.Form["Theme"];
                                    try { ss.UrlMode = (SiteUrlMode)int.Parse(Request.Form["UrlMode"]); }
                                    catch (Exception) { ss.UrlMode = SiteUrlMode.Rewrite; }
                                    ss.Management = Request.Form["Management"];
                                    ss.WapDomain = Request.Form["WapDomain"];
                                    ss.SubDomain = Request.Form["SubDomain"];
                                    ss.DataProvider = Request.Form["DataProvider"];
                                    try { ss.CacheMode = (CacheMode)int.Parse(Request.Form["CacheMode"]); }
                                    catch (Exception) { ss.CacheMode = CacheMode.Application; }
                                    ss.CacheProvider = Request.Form["CacheProvider"];
                                    ss.ResourcesUrl = Request.Form["ResourcesUrl"];
                                    ss.PassportUrl = Request.Form["PassportUrl"];
                                    ss.WapPassportUrl = Request.Form["WapPassportUrl"];
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "passport":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    PassportSection section = PassportSection.GetSection(config);
                                    section.CookieName = Request.Form["CookieName"];
                                    section.CookieDomain = Request.Form["CookieDomain"];
                                    section.CookieIV = Request.Form["CookieIV"];
                                    section.CookieKey = Request.Form["CookieKey"];
                                    section.MaxInvalidPasswordAttempts = GetInt32Value(Request.Form["MaxInvalidPasswordAttempts"], Utility.PassportMaxInvalidPasswordAttempts);
                                    section.PasswordAnswerAttemptLockoutDuration = GetInt32Value(Request.Form["PasswordAnswerAttemptLockoutDuration"], Utility.PassportPasswordAnswerAttemptLockoutDuration);
                                    try { section.Level = (PassportLevel)int.Parse(Request.Form["Level"]); }
                                    catch (Exception) { section.Level = PassportLevel.Low; }
                                    section.VerifyMail = GetBooleanValue(Request.Form["VerifyMail"]);
                                    section.VerifyMobile = GetBooleanValue(Request.Form["VerifyMobile"]);
                                    section.LoginWithCaptcha = GetBooleanValue(Request.Form["LoginWithCaptcha"]);
                                    section.RegisterWithCaptcha = GetBooleanValue(Request.Form["RegisterWithCaptcha"]);
                                    section.DefaultApproved = GetBooleanValue(Request.Form["DefaultApproved"]);
                                    section.DataProvider = Request.Form["DataProvider"];
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "cache":
                            CacheProvider.Current.Clear();
                            SetResult(() =>
                            {
                                WriteLog("CLEAR CACHE");
                            });
                            break;
                        case "captcha":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    CaptchaSection section = CaptchaSection.GetSection(config);
                                    section.Chars = Request.Form["Chars"];
                                    section.CookiePrefix = Request.Form["CookiePrefix"];
                                    section.CookieDomain = Request.Form["CookieDomain"];
                                    section.DefaultWidth = GetInt32Value(Request.Form["DefaultWidth"], Utility.CaptchaDefaultWidth);
                                    section.DefaultHeight = GetInt32Value(Request.Form["DefaultHeight"], Utility.CaptchaDefaultHeight);
                                    section.DefaultCount = GetInt32Value(Request.Form["DefaultCount"], Utility.CaptchaDefaultCount);
                                    section.Expiration = GetInt32Value(Request.Form["Expiration"], Utility.CaptchaExpiration);
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "smscaptcha":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    SMSCaptchaSection section = SMSCaptchaSection.GetSection(config);
                                    section.Chars = Request.Form["Chars"];
                                    section.DefaultCount = GetInt32Value(Request.Form["DefaultCount"], Utility.SMSCaptchaDefaultCount);
                                    section.TimeSpan = GetInt32Value(Request.Form["TimeSpan"], Utility.SMSCaptchaTimeSpan);
                                    section.Expiration = GetInt32Value(Request.Form["Expiration"], Utility.SMSCaptchaExpiration);
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "filesystem":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    FileSystemSection section = FileSystemSection.GetSection(config);
                                    section.Enable = Types.GetBooleanFromString(Request.Form["Enable"]);
                                    section.Path = Request.Form["Path"];
                                    section.Url = Request.Form["Url"];
                                    try { section.Mark = (ImageMarkType)int.Parse(Request.Form["Mark"]); }
                                    catch (Exception) { section.Mark = ImageMarkType.None; }
                                    section.Text = Request.Form["Text"];
                                    section.Region = (ImageMarkRegion)int.Parse(Request.Form["Region"]);
                                    try { section.Width = int.Parse(Request.Form["Width"]); }
                                    catch (Exception) { section.Width = 0; }
                                    try { section.Height = int.Parse(Request.Form["Height"]); }
                                    catch (Exception) { section.Height = 0; }
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "email":
                            try
                            {
                                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                SmtpSection section = (SmtpSection)config.GetSection("system.net/mailSettings/smtp");
                                section.From = Request.Form["UserName"];
                                section.Network.Host = Request.Form["Host"];
                                section.Network.Port = GetInt32Value(Request.Form["Port"], 25);
                                section.Network.UserName = Request.Form["UserName"];
                                section.Network.Password = Request.Form["Password"];
                                section.Network.EnableSsl = GetBooleanValue(Request.Form["EnableSsl"]);
                                config.Save();
                                SetResult(() =>
                                {
                                    WritePostLog("MOD");
                                });
                            }
                            catch (Exception ex)
                            {
                                SetResult(false, ex.Message);
                            }
                            break;
                        case "friendlink":
                            {
                                try
                                {
                                    System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                                    FriendLinkSection section = FriendLinkSection.GetSection(config);
                                    section.Enable = Types.GetBooleanFromString(Request.Form["Enable"]);
                                    section.Approved = Types.GetBooleanFromString(Request.Form["Approved"]);
                                    try { section.Mode = (FriendLinkMode)int.Parse(Request.Form["Mode"]); }
                                    catch (Exception) { section.Mode = FriendLinkMode.Text; }
                                    config.Save();
                                    SetResult(() =>
                                    {
                                        WritePostLog("MOD");
                                    });
                                }
                                catch (Exception ex)
                                {
                                    SetResult(false, ex.Message);
                                }
                            }
                            break;
                        case "robots":
                            try
                            {
                                string file = Server.MapPath("~/robots.txt");
                                File.WriteAllBytes(file, Encoding.UTF8.GetBytes(Request.Form["Robots"]));
                                SetResult(() =>
                                {
                                    WritePostLog("SUB");
                                });
                            }
                            catch (Exception ex)
                            {
                                SetResult(false, ex.Message);
                            }
                            break;
                        case "sitemap":
                            break;
                    }
                }
            }
        }

        public void Log(Arguments args)
        {
            int count = args.Count;
            if (count > 0)
            {
                if (CheckRight())
                {
                    string type = args[0];
                    int page = 1;
                    if (count > 1)
                        page = args.Get<int>(1);
                    if (page == 0)
                    {
                        DataActionHandler hander = null;
                        switch (type)
                        {
                            case "operation":
                                hander = new DataActionHandler(M.LogOperation.Clear);
                                break;
                            case "error":
                            case "sql":
                                break;
                        }
                        if (hander != null)
                        {
                            hander.Invoke(DataSource);
                            SetResult(() =>
                            {
                                WriteLog(string.Concat("clear ", type, " logs"));
                            });
                        }
                        else
                        {
                            NotFound();
                        }
                    }
                    else
                    {
                        DataGetPageHandler hander = null;
                        switch (type)
                        {
                            case "operation":
                                hander = new DataGetPageHandler(M.LogOperation.GetPage);
                                break;
                            case "error":
                            case "sql":
                                break;
                        }
                        if (hander != null)
                        {
                            SetResult(hander.Invoke(DataSource, page, 10, 11));
                        }
                        else
                        {
                            NotFound();
                        }
                    }
                }
            }
            else
            {
                if (CheckRight())
                {
                    if (CheckPost("log"))
                        NotFound();
                }
            }
        }

        public void Database(string type)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost(string.Concat("db_", type), new Action(() =>
                    {
                        this["Conns"] = WebConfigurationManager.ConnectionStrings;
                    })))
                    {
                        switch (type)
                        {
                            case "excute":
                                {
                                    try
                                    {
                                        DataTableEx table = new DataTableEx();
                                        using (DataSource ds = new DataSource(Request.Form["Conn"]))
                                            ds.ExecuteReader(table, Request.Form["sql"]);
                                        SetResult(true, () =>
                                        {
                                            WritePostLog("EXE");
                                        }, table);
                                    }
                                    catch (Exception ex)
                                    {
                                        SetResult(false, ex.Message);
                                    }
                                }
                                break;
                            case "vacuum":
                                {
                                    try
                                    {
                                        using (DataSource ds = new DataSource(Request.Form["Conn"]))
                                        {
                                            if (DbTable.Vacuum(ds))
                                            {
                                                SetResult(() =>
                                                {
                                                    WritePostLog("SUB");
                                                });
                                            }
                                            else
                                            {
                                                SetResult(-1);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SetResult(false, ex.Message);
                                    }
                                }
                                break;
                            case "backup":
                                {
                                    SetResult(-1);
                                }
                                break;
                            case "reduce":
                                {
                                    SetResult(-1);
                                }
                                break;
                        }
                    }
                }
            }
        }

        //public void Virtualdata(Arguments args)
        //{
        //    int count = args.Count;
        //    if (count > 0)
        //    {
        //        if (CheckAjax())
        //        {
        //            if (CheckRight())
        //            {
        //                string type = args[0];
        //                switch (type.Length)
        //                {
        //                    case 4:
        //                        if ("list".Equals(type, StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            int page = 1;
        //                            if (count > 1)
        //                                page = args.Get<int>(1);
        //                            SetResult(VM.VirtualDataTable.GetPage(DataSource, page, 10, 11));
        //                            return;
        //                        }
        //                        break;
        //                }
        //                NotFound();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (CheckAjax())
        //        {
        //            if (CheckRight())
        //            {
        //                if (CheckPost("vdata"))
        //                    NotFound();
        //            }
        //        }
        //    }
        //}

        private void AdminRole(Arguments args)
        {
            int count = args.Count;
            if (count > 0)
            {
                if (CheckRight())
                {
                    string type = args[0];
                    int num = 0;
                    if (count > 1)
                        num = args.Get<int>(1);
                    switch (type.Length)
                    {
                        case 3:
                            if ("add".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.AdminRole.Insert(DataSource, Request.Form["Name"], Request.Form["Rights"]), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            if ("del".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.AdminRole.Delete(DataSource, int.Parse(Request.Form["Id"])), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            //if ("get".Equals(type, StringComparison.OrdinalIgnoreCase))
                            //{
                            //    M.AdminRole role = M.AdminRole.GetById(DataSource, num);
                            //    SetResult(role);
                            //    return;
                            //}
                            if ("mod".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.AdminRole.Update(DataSource, int.Parse(Request.Form["Id"]), Request.Form["Name"], Request.Form["Rights"]), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            break;
                        case 4:
                            if ("list".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                SetResult(M.AdminRole.GetPage(DataSource, num, 10, 11));
                                return;
                            }
                            break;
                            //case 6:
                            //    if ("rights".Equals(type, StringComparison.OrdinalIgnoreCase))
                            //    {
                            //        SetResult(M.AdminRight.GetAll(DataSource));
                            //        return;
                            //    }
                            //    break;
                    }
                    NotFound();
                }
            }
            else
            {
                if (CheckRight())
                {
                    if (CheckPost("adminrole", () =>
                    {
                        this["Rights"] = M.AdminRight.GetAll(DataSource);
                    }))
                        NotFound();
                }
            }
        }
        private void AdminAdmin(Arguments args)
        {
            int count = args.Count;
            if (count > 0)
            {
                if (CheckRight())
                {
                    string type = args[0];
                    int num = 0;
                    if (count > 1)
                        num = args.Get<int>(1);
                    switch (type.Length)
                    {
                        case 3:
                            if ("add".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.Admin.Insert(DataSource, Request.Form["Name"], Request.Form["ConPassword"], int.Parse(Request.Form["RoleId"]), Request.Form["Email"]), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            if ("del".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.Admin.Delete(DataSource, int.Parse(Request.Form["Id"])), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            //if ("get".Equals(type, StringComparison.OrdinalIgnoreCase))
                            //{
                            //    SetResult(M.Admin.GetById(DataSource, num));
                            //    return;
                            //}
                            if ("mod".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsPost)
                                {
                                    SetResult(M.Admin.Update(DataSource, int.Parse(Request.Form["Id"]), Request.Form["ConPassword"], int.Parse(Request.Form["RoleId"]), Request.Form["Email"]), () =>
                                    {
                                        WritePostLog(type.ToUpper());
                                    });
                                    return;
                                }
                            }
                            break;
                        case 4:
                            if ("list".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                SetResult(M.Admin.GetPage(DataSource, num, 10, 11));
                                return;
                            }
                            break;
                            //case 5:
                            //    if ("roles".Equals(type, StringComparison.OrdinalIgnoreCase))
                            //    {
                            //        SetResult(M.AdminRole.GetAll(DataSource));
                            //        return;
                            //    }
                            //    break;
                            //case 8:
                            //    if ("allroles".Equals(type, StringComparison.OrdinalIgnoreCase))
                            //    {
                            //        SetJavascript("adminRoles", M.AdminRole.GetAllNames(DataSource));
                            //        return;
                            //    }
                            //    break;
                    }
                    NotFound();
                }
            }
            else
            {
                if (CheckRight())
                {
                    if (CheckPost("admin", () =>
                    {
                        this["Roles"] = M.AdminRole.GetAll(DataSource);
                    }))
                        NotFound();
                }
            }
        }
        public void Admin(string channel, Arguments args)
        {
            switch (channel)
            {
                case "admin": AdminAdmin(args); break;
                case "role": AdminRole(args); break;
            }
        }

        private string ToJson(NameValueCollection nvc)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string key in nvc.Keys)
            {
                if (i++ > 0)
                    sb.Append(',');
                sb.Append(string.Concat((new JsonString(key)).ToJsonString(), ":", (new JsonString(nvc[key])).ToJsonString()));
            }
            return sb.ToString();
        }

        internal bool CheckAjax()
        {
            if (IsAjax)
                return true;
            NotFound();
            return false;
        }
        internal bool CheckRight()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
            {
                if (M.Admin.HasRight(DataSource, (int)User.Identity.AdminRoleId, Application.Action))
                    return true;
            }
            Unauthorized();
            return false;
        }
        private bool CheckPost(string key, Action action = null)
        {
            return CheckPost(Namespace, key, action, GetType(), this);
        }
        internal bool CheckPost(string ns, string key, Action action, Type type, IRenderer renderer)
        {
            if (IsPost)
                return true;
            if (action != null)
                action();
            WriteLog(string.Concat("VIEW ", Url));
            RenderTemplate(ns, string.Concat(key, ".html"), type, renderer);
            return false;
        }
        internal void WritePostLog(string name)
        {
            StringBuilder sb = new StringBuilder(name);
            if (Request.Form.Count > 0)
            {
                sb.Append(' ');
                sb.Append(ToJson(Request.Form));
            }
            WriteLog(sb.ToString());
        }

        public void Menus()
        {
            if (User != null && User.Identity.IsAuthenticated && User.Identity.IsAdmin)
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/x-javascript";
                Response.Write(ManagementMenus.GetAll(this));
            }
            else
            {
                Unauthorized();
            }
        }

        public void Default(string name, Arguments nvc)
        {
            ManagementController ma = CreateAction(Context, name);
            if (ma != null)
            {
                ma.SetManagement(this);
                string action = Utility.DefaultAction;
                if (nvc.Count > 0)
                {
                    action = nvc[0];
                    nvc.SetIndex(1);
                }
                MethodInfo method = ma.GetType().GetMethod(action, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
                if (method == null)
                    throw new DebugException(string.Concat("Action \"", name, ".", action, "\" can not defined"));

                int argsn = 0;
                object[] args = null;
                ParameterInfo[] ps = method.GetParameters();
                if (ps != null)
                    argsn = ps.Length;
                bool hasArgs = (ps.Length > 0 && ps[ps.Length - 1].ParameterType == TType<Arguments>.Type);
                if (nvc.Count > argsn && !hasArgs)
                {
                    StringBuilder sb = new StringBuilder(string.Concat("Action \"", name, ".", action, "("));
                    for (int i = 0; i < ps.Length; ++i)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        sb.Append(ps[i].ParameterType.Name);
                    }
                    sb.Append(")\" 参数不匹配");
                    throw new DebugException(sb.ToString());
                }
                if (argsn > 0)
                {
                    args = new object[argsn];
                    for (int i = 0; i < argsn; ++i)
                    {
                        if (hasArgs && i == (argsn - 1))
                        {
                            nvc.SetIndex(i);
                            args[i] = nvc;
                        }
                        else
                        {
                            args[i] = Application.FormatParameter(ps[i], nvc[i]);
                        }
                    }
                }

                method.Invoke(ma, args);
            }
            else
            {
                NotFound();
            }
        }
        private static ManagementController CreateAction(HttpContext context, string name)
        {
            name = name.ToUpper();
            CacheTable<Type> cache = new CacheTable<Type>(Utility.ActionTypeCacheName);
            Type type = cache[name];
            if (type == null)
            {
                int index;
                string asm;
                DirectoryInfo dir = new DirectoryInfo(context.Server.MapPath("~/Bin"));
                foreach (FileInfo file in dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    index = file.Name.LastIndexOf('.');
                    asm = file.Name.Substring(0, index);
                    type = Type.GetType(string.Concat(asm, ".Management.", name, ",", asm), false, true);
                    if (type != null)
                    {
                        cache[name] = type;
                        break;
                    }
                }
            }
            if (type != null)
                return Activator.CreateInstance(type) as ManagementController;
            return null;
        }
    }
}
