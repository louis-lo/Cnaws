using System;
using System.IO;
using System.Web;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Threading;

namespace Cnaws.Web
{
    public sealed class Application
    {
        private Settings _settings;
        private HttpContext _context;
        private bool _isWap;
        private string _action;
        private Controller _controller;
        private string _ctlName;
        private string _actName;

        internal Application(HttpContext context, bool isWap)
        {
            _settings = Settings.Instance;
            _context = context;
            _isWap = isWap;
            _action = string.Empty;
            _controller = null;
            _ctlName = string.Empty;
            _actName = string.Empty;
            context.Items[Utility.ApplicationItemName] = this;
        }

        public static Application Current
        {
            get { return (Application)HttpContext.Current.Items[Utility.ApplicationItemName]; }
        }

        public Settings Settings
        {
            get { return _settings; }
        }
        internal HttpContext Context
        {
            get { return _context; }
        }
        internal bool IsWap
        {
            get { return _isWap; }
        }
        public string Action
        {
            get { return _action; }
        }
        public Controller Controller
        {
            get { return _controller; }
        }
        public string ControllerName
        {
            get { return _ctlName; }
        }
        public string ActionName
        {
            get { return _actName; }
        }

        private void Reset()
        {
            _controller = null;
            _action = string.Empty;
        }
        internal void Run(string url)
        {
            Run(new UrlParse(url));
        }
        internal void Run(UrlParse parse)
        {
            Reset();
            
            if (parse.Segments.Count > 0)
                _ctlName = parse.Segments[0];
            else
                _ctlName = Utility.DefaultController;
            _controller = CreateController(_ctlName);
            if (_controller == null)
                throw new DebugException(string.Concat("Controller \"", _ctlName, "\" can not defined"));

            try
            {
                _controller.InitController(this, parse.Url, parse.ExtensionType, parse.Extension);
                
                if (parse.Segments.Count > 1)
                    _actName = parse.Segments[1];
                else
                    _actName = Utility.DefaultAction;

                bool isDefault = false;
                MethodInfo method = _controller.GetType().GetMethod(_actName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
                if (method == null)
                {
                    method = _controller.GetType().GetMethod(Utility.NormalAction, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
                    if (method != null)
                        isDefault = true;
                    else
                        throw new DebugException(string.Concat("Action \"", _ctlName, ".", _actName, "\" can not defined"));
                }

                Type type;
                object[] attrs = method.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr != null)
                    {
                        type = attr.GetType();
                        if (TType<ActionMethodSelectorAttribute>.Type.IsAssignableFrom(type))
                        {
                            ActionMethodSelectorAttribute selector = (ActionMethodSelectorAttribute)attr;
                            if (!selector.IsValidForRequest(_controller, method))
                            {
                                if (TType<AuthorizeAttribute>.Type.IsAssignableFrom(type))
                                    _controller.Unauthorized(((AuthorizeAttribute)attr).Redirect);
                                else
                                    _controller.NotFound();
                                goto EndLine;
                            }
                        }
                    }
                }

                int argsn = 0;
                object[] args = null;
                ParameterInfo[] ps = method.GetParameters();
                if (ps != null)
                    argsn = ps.Length;
                bool hasArgs = (ps.Length > 0 && ps[ps.Length - 1].ParameterType == TType<Arguments>.Type);
                if (!isDefault && (parse.Segments.Count - 2) > argsn && !hasArgs)
                {
                    StringBuilder sb = new StringBuilder(string.Concat("Action \"", _ctlName, ".", _actName, "("));
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
                    int x;
                    string s;
                    int setp = 2;
                    int begin = 0;
                    args = new object[argsn];
                    if (isDefault)
                    {
                        setp = 1;
                        begin = 1;
                        args[0] = _actName;
                    }
                    for (int i = begin; i < argsn; ++i)
                    {
                        x = i + setp;
                        if (hasArgs && i == (argsn - 1))
                        {
                            args[i] = new Arguments(parse.Segments, x);
                        }
                        else
                        {
                            if (x < parse.Segments.Count)
                                s = parse.Segments[x];
                            else
                                s = null;
                            args[i] = FormatParameter(ps[i], s);
                        }
                    }
                }
                _action = string.Concat(_ctlName, ".", _actName).ToLower();
                method.Invoke(_controller, args);
                EndLine:
                _controller.End();
            }
            finally
            {
                ReleaseController();
            }
        }
        private void ReleaseController()
        {
            if (_controller != null)
            {
                _controller.Dispose();
                _controller = null;
            }
        }

        internal void RenderError(int status = 500)
        {
            HttpException e;
            if (Context.Error != null)
            {
                Exception ex = Context.Error.GetBaseException();
                if(ex is JsonResultException)
                {
                    Controller.SetResult(Context.Response, ((JsonResultException)ex).Code);
                    Context.Response.End();
                    return;
                }
                if (ex is DebugException)
                {
                    if (!Settings.Debug)
                    {
                        e = new HttpException(404, HttpWorkerRequest.GetStatusDescription(404));
                        goto EndLine;
                    }
                }
                if (ex is HttpException)
                    e = (HttpException)ex;
                else
                    e = new HttpException(500, ex.Message, ex);
            }
            else
            {
                e = new HttpException(status, HttpWorkerRequest.GetStatusDescription(status));
            }
            EndLine:
            if (CustomErrors.IsCustom(this))
                CustomErrors.RenderCustomError(this, e.ErrorCode);
            else
                CustomErrors.ThrowException(this, e);
        }

        private Controller CreateController(string name)
        {
            name = name.ToUpper();
            CacheTable<Type> cache = new CacheTable<Type>(Utility.ControllerTypeCacheName);
            Type type = cache[name];
            if (type == null)
            {
                int index;
                string asm;
                DirectoryInfo dir = new DirectoryInfo(Context.Server.MapPath("~/Bin"));
                FileInfo[] files = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    index = file.Name.LastIndexOf('.');
                    asm = file.Name.Substring(0, index);
                    type = Type.GetType(string.Concat(asm, ".Controllers.Extension.", name, ',', asm), false, true);
                    if (type != null)
                    {
                        cache[name] = type;
                        break;
                    }
                }
                if (type == null)
                {
                    foreach (FileInfo file in files)
                    {
                        index = file.Name.LastIndexOf('.');
                        asm = file.Name.Substring(0, index);
                        type = Type.GetType(string.Concat(asm, ".Controllers.", name, ',', asm), false, true);
                        if (type != null)
                        {
                            cache[name] = type;
                            break;
                        }
                    }
                }
            }
            if (type != null)
                return Activator.CreateInstance(type) as Controller;
            return null;
        }
        internal static object FormatParameter(ParameterInfo p, string s)
        {
            if (p.ParameterType == TType<string>.Type)
                return s;

            if (string.IsNullOrEmpty(s))
            {
                if (p.DefaultValue != System.DBNull.Value)
                    return p.DefaultValue;
                return p.ParameterType.GetDefaultValue();
            }

            return p.ParameterType.GetObjectFromString(s);
        }
    }
}
