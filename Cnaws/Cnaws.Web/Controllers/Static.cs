using System;
using System.Reflection;
using Cnaws.Json;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Web
{
    public sealed class StaticCallException : ArgumentException
    {
        private int _code;

        public StaticCallException(int code)
        {
            _code = code;
        }
        public StaticCallException(int code, string target)
            : base(null, target)
        {
            _code = code;
        }

        public int Code
        {
            get { return _code; }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class StaticCallAttribute : Attribute
    {
    }

    public interface IStaticCallResult
    {
        int Code { get; set; }
        object Data { get; set; }
    }
    internal sealed class StaticCallResultSingle : IStaticCallResult
    {
        private int code;

        public StaticCallResultSingle(int code)
        {
            this.code = code;
        }

        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        object IStaticCallResult.Data
        {
            get { return null; }
            set { }
        }

        public override string ToString()
        {
            return JsonValue.Serialize<StaticCallResultSingle>(this);
        }
    }
    public sealed class StaticCallResult: IStaticCallResult
    {
        private int code;
        private object data;

        public StaticCallResult()
        {
            code = 0;
            data = null;
        }
        public StaticCallResult(int code, object data)
        {
            this.code = code;
            this.data = data;
        }

        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public override string ToString()
        {
            if (data != null)
                return JsonValue.Serialize<StaticCallResult>(this);
            return GetJson(code);
        }

        public static string GetJson(int code, object data = null)
        {
            if (data != null)
                return (new StaticCallResult(code, data)).ToString();
            return (new StaticCallResultSingle(code)).ToString();
        }
        public static string GetJson<T>(int code, T data)
        {
            return (new StaticCallResult<T>(code, data)).ToString();
        }
    }

    public sealed class StaticCallResult<T> : IStaticCallResult
    {
        private int code;
        private T data;

        public StaticCallResult()
        {
            code = 0;
            data = default(T);
        }
        public StaticCallResult(int code, T data)
        {
            this.code = code;
            this.data = data;
        }

        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        object IStaticCallResult.Data
        {
            get { return data; }
            set
            {
                if (value == null)
                    data = default(T);
                else
                    data = (T)value;
            }
        }

        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        public override string ToString()
        {
            if (((object)data) != null)
                return JsonValue.Serialize<StaticCallResult<T>>(this);
            return StaticCallResult.GetJson(code);
        }
    }
}

namespace Cnaws.Web.Controllers
{
    public sealed class Static : Controller
    {
        private void Execute(string v, string t, string m, Arguments nvc)
        {
            IStaticCallResult result;
            try
            {
                Type type = t.ToType(false);
                if (type == null)
                    throw new ArgumentException(string.Concat("找不到类型 ", t));
                MethodInfo method = type.GetMethod(m, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
                if (type == null)
                    throw new ArgumentException(string.Concat("在类型 ", t, " 中找不到方法 ", m));
                object[] att = method.GetCustomAttributes(TType<StaticCallAttribute>.Type, true);
                if (att == null || att.Length <= 0)
                    throw new ArgumentException(string.Concat("无权限访问类型 ", t, " 中的方法 ", m));
                result = new StaticCallResult();
                ParameterInfo[] ps = method.GetParameters();
                object[] args = null;
                if (ps != null && ps.Length > 0)
                {
                    args = new object[ps.Length];
                    if ("POST".Equals(Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                    {
                        ParameterInfo p;
                        for (int i = 0; i < ps.Length; ++i)
                        {
                            p = ps[i];
                            args[i] = Application.FormatParameter(p, Request.Form[p.Name]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ps.Length; ++i)
                            args[i] = nvc.Get(i, ps[i].ParameterType);
                    }

                }
                result.Data = method.Invoke(null, args);
                result.Code = 0;
            }
            catch (StaticCallException ex)
            {
                result = new StaticCallResult<string>();
                result.Data = ex.ParamName;
                result.Code = ex.Code;
            }
            catch (Exception ex)
            {
                StaticCallException e = ex.GetBaseException() as StaticCallException;
                if (e != null)
                {
                    result = new StaticCallResult<string>();
                    result.Data = e.ParamName;
                    result.Code = e.Code;
                }
                else
                {
                    result = new StaticCallResult<string>();
                    result.Data = string.Concat(ex.Message, "\r\n", ex.StackTrace);
                    result.Code = -1;
                }
            }

            Response.ContentType = "application/x-javascript";
            if (string.IsNullOrEmpty(v))
                Response.Write(result.ToString());
            else
                Response.Write(string.Concat("var ", v, "=", result.ToString(), ";"));
        }
        public void Call(string type, string method, Arguments args)
        {
            Execute(null, type, method, args);
        }
        public void Load(string name, string type, string method, Arguments args)
        {
            Execute(name, type, method, args);
        }
    }
}
