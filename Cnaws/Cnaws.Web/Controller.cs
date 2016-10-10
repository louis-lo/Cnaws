using System;
using System.IO;
using System.Web;
using System.Text;
using Cnaws.Data;
using Cnaws.Web.Templates;
using Cnaws.Web.Templates.Parser;
using Cnaws.Net;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using Cnaws.ExtensionMethods;
using Cnaws.Web.Configuration;

namespace Cnaws.Web
{
    public interface IRenderer
    {
        void Render(string text, string path);
    }

    public sealed class TempContext
    {
        private VariableScope _vars;

        public TempContext(string file, Encoding charset)
        {
            _vars = new VariableScope();
            TemplateContext tempContext = new TemplateContext(file, charset, true, _vars);
            Template temp = new Template(tempContext, Resources.Load(tempContext.CurrentPath, tempContext.Charset));
            temp.Eval();
        }

        public object this[string name]
        {
            get { return _vars[name]; }
        }
    }

    public abstract class Controller : IDisposable, IRenderer
    {
        private bool disposed = false;

        private Application _app;
        private string _url;
        private ExtensionType _extType;
        private string _ext;
        private VariableScope _vars;

#if (DEBUG)
        public static List<ApiMethod> ApiMethods = new List<ApiMethod>();
        public static Dictionary<string, Type> ApiTypes = new Dictionary<string, Type>();
        public sealed class ApiMethod
        {
            public string NameSpace;
            public string Name;
            public string Summary;
            public List<ApiArgument> Arguments;
            public List<ApiResult> Results;

            internal ApiMethod(string ns, string name, string summary)
            {
                NameSpace = ns;
                Name = name;
                Summary = summary;
                Arguments = new List<ApiArgument>();
                Results = new List<ApiResult>();
            }

            public ApiMethod AddArgument(string name, Type value, string summary)
            {
                Arguments.Add(new ApiArgument(name, value, summary));
                return this;
            }
            public ApiMethod AddResult(Type data, string datas)
            {
                return AddResult(data != null, data, datas);
            }
            public ApiMethod AddResult(bool code, Type data = null, string datas = null)
            {
                return AddResult(code ? -200 : -500, code ? "成功" : "代码错误", data, datas);
            }
            public ApiMethod AddResult(int code, string codes, Type data = null, string datas = null)
            {
                Results.Add(new ApiResult(code, codes, data, datas));
                return this;
            }

            public string GetUrl()
            {
                return string.Concat('/', NameSpace, '/', Name, Utility.DefaultExt).ToLower();
            }
        }
        public sealed class ApiArgument
        {
            public string Name;
            public Type Value;
            public string Summary;

            internal ApiArgument(string name, Type value, string summary)
            {
                Name = name;
                Value = value;
                Summary = summary;
                if (value != null)
                    ApiTypes[value.FullName] = value;
            }
        }
        public sealed class ApiResult
        {
            public int Code;
            public string CodeSummary;
            public Type Data;
            public string DataSummary;

            internal ApiResult(int code, string codes, Type data, string datas)
            {
                Code = code;
                CodeSummary = codes;
                Data = data;
                DataSummary = datas;
                if (data != null)
                    ApiTypes[data.FullName] = data;
            }
        }
        protected static ApiMethod AddApiMethod(string ns, string name, string summary)
        {
            ApiMethod method = new ApiMethod(ns, name, summary);
            ApiMethods.Add(method);
            return method;
        }
#endif

        public Controller()
        {
            _app = null;
            _url = null;
            _extType = ExtensionType.Unknown;
            _ext = null;
            _vars = null;
        }

        //public virtual bool IsReusable
        //{
        //    get { return false; }
        //}
        public Application Application
        {
            get { return _app; }
        }
        public HttpContext Context
        {
            get { return Application.Context; }
        }
        public HttpRequest Request
        {
            get { return Context.Request; }
        }
        public HttpResponse Response
        {
            get { return Context.Response; }
        }
        public HttpServerUtility Server
        {
            get { return Context.Server; }
        }
        public virtual bool IsWap
        {
            get { return Application.IsWap; }
        }
        public string Url
        {
            get { return _url; }
        }
        internal ExtensionType ExtensionType
        {
            get { return _extType; }
        }
        internal string Extension
        {
            get { return _ext; }
        }
        public object this[string name]
        {
            get { return _vars[name]; }
            set { _vars[name] = value; }
        }
        public string SubDomain
        {
            get
            {
                object value = Context.Items[Utility.SubDomainItemName];
                if (value != null)
                    return (string)value;
                return null;
            }
        }

        public bool IsPost
        {
            get { return string.Equals("POST", Request.HttpMethod); }
        }
        public bool IsAjax
        {
            get
            {
                return "XMLHttpRequest".Equals(Request.ServerVariables["HTTP_X_REQUESTED_WITH"], StringComparison.OrdinalIgnoreCase);
            }
        }
        internal static string GetClientIp(HttpContext context = null)
        {
            string temp;
            string realip = null;
            if (context == null)
                context = HttpContext.Current;
            if (!string.IsNullOrEmpty(temp = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
            {
                string s;
                string[] ipArray = temp.Split(',');
                foreach (string rs in ipArray)
                {
                    s = rs.Trim();
                    if (s != "unknown")
                    {
                        realip = s;
                        break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(temp = context.Request.ServerVariables["HTTP_CLIENT_IP"]))
            {
                realip = temp;
            }
            else
            {
                realip = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            return realip;
        }
        public string ClientIp
        {
            get { return GetClientIp(Context); }
        }

        public PassportPrincipal User
        {
            get { return Context.User as PassportPrincipal; }
        }

        public bool IsLogin
        {
            get
            {
                PassportPrincipal user = User;
                if (user != null && user.Identity != null && user.Identity.IsAuthenticated && !user.Identity.IsAdmin)
                    return true;
                return false;
            }
        }

        private static bool HasItem(string[] array, string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (array != null && array.Length > 0)
            {
                foreach (string item in array)
                {
                    if (item != null && item.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }
        public bool ClientAccept(string type)
        {
            return HasItem(Request.AcceptTypes, type);
        }
        protected bool IsHtml()
        {
            return ClientAccept("text/html");
        }

        public void Redirect(string url, bool moved = false)
        {
            Response.StatusCode = moved ? 301 : 302;
            Response.RedirectLocation = url;
            if ((url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)) || ((url.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("file:", StringComparison.OrdinalIgnoreCase)) || url.StartsWith("news:", StringComparison.OrdinalIgnoreCase)))
                url = HttpUtility.HtmlAttributeEncode(url);
            else
                url = HttpUtility.HtmlAttributeEncode(HttpUtility.UrlEncode(url));
            Response.Write("<html><head><title>Object moved</title></head><body>\r\n");
            Response.Write("<h2>Object moved to <a href=\"" + url + "\">here</a>.</h2>\r\n");
            Response.Write("</body></html>\r\n");
            End();
        }
        public void Refresh(string url)
        {
            Response.ContentType = "text/html";
            Response.Write(@"<!doctype html>
<html>
<head>
<title>跳转</title>
<meta http-equiv=""refresh"" content=""0; url=");
            Response.Write(url);
            Response.Write(@""" />
</head>
<body style=""font-size:14px;padding:10px;text-align:center;"">
如果您的浏览器没有跳转，请<a href=""");
            Response.Write(url);
            Response.Write(@""">点击这里</a>。
</body>
</html>");
            End();
        }
        public void End()
        {
            try { Response.End(); }
            catch (Exception) { }
        }

        private string GetModeUrl(string path)
        {
            //if (Settings.Instance.UrlMode == Configuration.SiteUrlMode.Dynamic)
            //    return string.Concat(Settings.Instance.RootUrl, Utility.RewriteUrlPath, path);
            if (path.EndsWith("/"))
            {
                if (path.Length > 1)
                    return string.Concat(Settings.Instance.RootUrl, path.Substring(1));
                return Settings.Instance.RootUrl;
            }
            return string.Concat(Settings.Instance.RootUrl, path.Substring(1), Utility.DefaultExt);
        }
        protected virtual string GetVirtualUrl(string path)
        {
            string baseUrl = _url;
            int index = baseUrl.LastIndexOf('/');
            baseUrl = baseUrl.Substring(0, index + 1);
            return GetModeUrl(string.Concat(baseUrl, path));
        }
        public string GetUrl(params string[] path)
        {
            string url = string.Concat(path);
            if (url.StartsWith("/"))
                return GetModeUrl(url);

            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                return url;

            return GetVirtualUrl(url);
        }
        private string GetModeRes(string path)
        {
            string url = Settings.Instance.ResourcesUrl;
            if (string.IsNullOrEmpty(url))
                return string.Concat(Settings.Instance.RootUrl, path.Substring(1));
            return string.Concat(url, path);
        }
        protected virtual string GetVirtualRes(string path)
        {
            string url = Settings.Instance.ResourcesUrl;
            if (string.IsNullOrEmpty(url))
                return string.Concat(Settings.Instance.ThemeUrl, path);
            return string.Concat(url, Settings.Instance.ThemeUrl, path);
        }
        public string GetRes(params string[] path)
        {
            string url = string.Concat(path);
            if (url.StartsWith("/"))
                return GetModeRes(url);

            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                return url;

            return GetVirtualRes(url);
        }
        public string GetFileSystemUrl(params string[] path)
        {
            FileSystemSection section = FileSystemSection.GetSection();
            if (!section.Enable && !string.IsNullOrEmpty(section.Url))
            {
                string[] args = new string[path.Length + 3];
                args[0] = section.Url;
                args[1] = "/filesystem/";
                Array.Copy(path, 0, args, 2, path.Length);
                args[args.Length - 1] = string.Concat(Utility.DefaultExt, "?token=", HttpUtility.UrlEncode(User.Identity.GetToken()));
                return string.Concat(args);
            }
            return GetUrl("/filesystem/");
        }
        public string GetAutoPassportUrl()
        {
            if (IsWap && !string.IsNullOrEmpty(Settings.Instance.WapPassportUrl))
                return Settings.Instance.WapPassportUrl;
            return Settings.Instance.PassportUrl;
        }
        public string GetPassportUrl(params string[] path)
        {
            string url = GetAutoPassportUrl();
            if (!string.IsNullOrEmpty(url))
            {
                string[] args = new string[path.Length + 2];
                args[0] = url;
                Array.Copy(path, 0, args, 1, path.Length);
                args[args.Length - 1] = Utility.DefaultExt;
                return string.Concat(args);
            }
            return GetUrl(path);
        }

        protected virtual void OnInitController()
        {

        }
        private void InitVariableScope()
        {
            _vars = new VariableScope();
            _vars["this"] = this;
            _vars["isset"] = new FuncHandler((args) =>
            {
                return _vars.ContainsKey((string)args[0]);
            });
            _vars["url"] = new FuncHandler((args) =>
            {
                return GetUrl(Array.ConvertAll(args, new Converter<object, string>((x) =>
                {
                    return x.ToString();
                })));
            });
            _vars["res"] = new FuncHandler((args) =>
            {
                return GetRes(Array.ConvertAll(args, new Converter<object, string>((x) =>
                {
                    return x.ToString();
                })));
            });
            _vars["urlencode"] = new FuncHandler((args) =>
            {
                return HttpUtility.UrlEncode(Convert.ToString(args[0]));
            });
            _vars["filesystem"] = new FuncHandler((args) =>
            {
                return GetFileSystemUrl(Array.ConvertAll(args, new Converter<object, string>((x) =>
                {
                    return x.ToString();
                })));
            });
            _vars["passport"] = new FuncHandler((args) =>
            {
                return GetPassportUrl(Array.ConvertAll(args, new Converter<object, string>((x) =>
                {
                    return x.ToString();
                })));
            });
            _vars["now"] = DateTime.Now;
            //_vars["ext"] = (Settings.Instance.UrlMode != Configuration.SiteUrlMode.Dynamic) ? Utility.DefaultExt : string.Empty;
            _vars["ext"] = Utility.DefaultExt;
        }

        internal void InitController(Application app, string url, ExtensionType extType, string ext)
        {
            _app = app;
            _url = url;
            _extType = extType;
            _ext = ext;
            InitVariableScope();
            Response.ContentEncoding = Settings.Instance.ResponseEncoding;
            Response.ContentType = "text/html";
            OnInitController();
        }
        public void InitController(Controller controller)
        {
            _app = controller._app;
            _url = controller._url;
            _extType = controller._extType;
            _ext = controller._ext;
            _vars = controller._vars;
            Response.ContentEncoding = controller.Response.ContentEncoding;
            Response.ContentType = controller.Response.ContentType;
            OnInitController();
        }
        public void Render(string path)
        {
            TemplateContext tempContext = new TemplateContext(Path.Combine(Settings.Instance.ThemePath, IsWap ? Utility.WapDir : Utility.WwwDir, path), Settings.Instance.FileEncoding, true/*, _url, Settings.Instance.ThemeUrl*/, _vars);
            Template temp = new Template(tempContext, string.Concat("${load(\"", Utility.ConfigDirP, "site.config\")}", Resources.Load(tempContext.CurrentPath, tempContext.Charset)));
            temp.Render(Response.Output);
        }
        public void Render(string text, string path)
        {
            TemplateContext tempContext = new TemplateContext(Path.Combine(Settings.Instance.ThemePath, IsWap ? Utility.WapDir : Utility.WwwDir, path), Settings.Instance.FileEncoding, true/*, _url, Settings.Instance.ThemeUrl*/, _vars);
            Template temp = new Template(tempContext, string.Concat("${load(\"", Utility.ConfigDirP, "site.config\")}", text));
            temp.Render(Response.Output);
        }

        public TempContext LoadConfig(string file)
        {
            return new TempContext(Server.MapPath(string.Concat(Utility.ConfigDir, file)), Settings.Instance.FileEncoding);
        }

        internal static void SetResult(HttpResponse response, int code, object value = null)
        {
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/x-javascript";
            response.Write(StaticCallResult.GetJson(code, value));
        }
        protected internal virtual void SetResult(int code, object value = null)
        {
            SetResult(Response, code, value);
        }
        protected internal virtual void SetResult(bool result, object value = null)
        {
            SetResult(result ? -200 : -500, value);
        }
        protected internal virtual void SetResult(object value)
        {
            SetResult(value != null, value);
        }
        protected internal virtual void SetResult(DataStatus status)
        {
            SetResult((int)status);
        }
        protected internal virtual void SetResult(DataStatus status, Action action, object value = null)
        {
            if (status == DataStatus.Success && action != null)
                action();
            SetResult((int)status, value);
        }
        protected internal virtual void SetResult(bool result, Action action, object value = null)
        {
            if (result && action != null)
                action();
            SetResult(result ? -200 : -500, value);
        }
        protected internal virtual void SetResult(Action action)
        {
            SetResult(true, action);
        }
        protected internal virtual void SetResult<T>(T module, Action<T> action) where T : Module
        {
            bool result = (module != null);
            if (result && action != null)
                action(module);
            SetResult(result ? -200 : -500, module);
        }
        protected internal virtual void SetJavascript(string name, object data)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/x-javascript";
            Response.Write(string.Concat("var ", name, "=", StaticCallResult.GetJson((data != null) ? -200 : -500, data), ";"));
        }
        protected internal virtual void SetJsonp(object data)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/x-javascript";
            Response.Write(string.Concat("callback(", StaticCallResult.GetJson((data != null) ? -200 : -500, data), ");"));
        }

        protected internal virtual void NotFound()
        {
            if (IsHtml())
                Application.RenderError(404);
            else
                SetResult(-404);
        }
        protected virtual string GetLoginUrl()
        {
            return string.Concat(GetPassportUrl("/login"), "?target=", HttpUtility.UrlEncode(Request.Url.ToString()));
        }
        protected internal virtual void Unauthorized(bool redirect = false)
        {
            if (redirect)
            {
                Redirect(GetLoginUrl());
            }
            else
            {
                if (IsHtml())
                    Application.RenderError(401);
                else
                    SetResult(-401);
            }
        }

        protected bool SendMail(MailMessage mm)
        {
            SmtpSection section = (SmtpSection)WebConfigurationManager.GetSection("system.net/mailSettings/smtp");
            SmtpClient client = new SmtpClient(section.Network.Host, section.Network.Port);
            return client.Send(mm, section.Network.UserName, section.Network.Password);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        private void DoDispose(bool disposing)
        {
            if (!disposed)
            {
                Dispose(disposing);
                disposed = true;
            }
        }
        public void Dispose()
        {
            DoDispose(true);
            GC.SuppressFinalize(this);
        }
        ~Controller()
        {
            DoDispose(false);
        }
    }

    public interface IDataController
    {
        string DataProvider { get; }
        DataSource DataSource { get; }
    }

    public abstract class DataController : Controller, IDataController
    {
        private DataSource _ds;

        public DataController()
        {
            _ds = null;
        }

        public virtual string DataProvider
        {
            get { return Settings.Instance.DataProvider; }
        }

        public DataSource DataSource
        {
            get
            {
                if (_ds == null)
                {
                    _ds = new DataSource(DataProvider);
                    _ds.Controller = this;
                }
                return _ds;
            }
        }

        public void InitController(DataController controller)
        {
            base.InitController(controller);
            if (controller._ds != null)
            {
                _ds = new DataSource(controller._ds);
                _ds.Controller = this;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                    _ds = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    public abstract class PassportController : DataController
    {
    }
}