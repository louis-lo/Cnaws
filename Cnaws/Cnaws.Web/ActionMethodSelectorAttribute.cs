using System;
using System.Web;
using System.Reflection;
using System.Collections.Generic;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Web
{
    [Flags]
    public enum HttpVerbs
    {
        All = 0,
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8,
        Head = 16
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ActionMethodSelectorAttribute : Attribute
    {
        protected ActionMethodSelectorAttribute()
        {
        }

        public abstract bool IsValidForRequest(Controller controller, MethodInfo methodInfo);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AcceptVerbsAttribute : ActionMethodSelectorAttribute
    {
        private HttpVerbs _verbs;

        public AcceptVerbsAttribute(params string[] verbs)
        {
            _verbs = HttpVerbs.All;
            if (verbs != null && verbs.Length > 0)
            {
                foreach (string verb in verbs)
                    _verbs |= Parse(verb);
            }
        }
        public AcceptVerbsAttribute(HttpVerbs verbs)
        {
            _verbs = verbs;
        }

        private HttpVerbs Parse(string s)
        {
            if (s == null)
                throw new ArgumentNullException();
            return (HttpVerbs)Enum.Parse(TType<HttpVerbs>.Type, s, true);
        }

        public override bool IsValidForRequest(Controller controller, MethodInfo methodInfo)
        {
            if (_verbs == HttpVerbs.All)
                return true;
            return (_verbs & Parse(controller.Request.HttpMethod)) != 0;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpGetAttribute : AcceptVerbsAttribute
    {
        public HttpGetAttribute()
            : base(HttpVerbs.Get)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpPostAttribute : AcceptVerbsAttribute
    {
        public HttpPostAttribute()
            : base(HttpVerbs.Post)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpHeadAttribute : AcceptVerbsAttribute
    {
        public HttpHeadAttribute()
            : base(HttpVerbs.Head)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpDeleteAttribute : AcceptVerbsAttribute
    {
        public HttpDeleteAttribute()
            : base(HttpVerbs.Delete)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpPutAttribute : AcceptVerbsAttribute
    {
        public HttpPutAttribute()
            : base(HttpVerbs.Put)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeAttribute : ActionMethodSelectorAttribute
    {
        private bool _redirect;

        public AuthorizeAttribute(bool redirect = false)
        {
            _redirect = redirect;
        }

        public bool Redirect
        {
            get { return _redirect; }
        }

        public override bool IsValidForRequest(Controller controller, MethodInfo methodInfo)
        {
            if (controller.User == null)
                return false;
            if (controller.User.Identity == null)
                return false;
            return controller.User.Identity.IsAuthenticated;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public AdminAuthorizeAttribute()
        {
        }

        public override bool IsValidForRequest(Controller controller, MethodInfo methodInfo)
        {
            if (controller.User == null)
                return false;
            if (controller.User.Identity == null)
                return false;
            if (!controller.User.Identity.IsAuthenticated)
                return false;
            return controller.User.Identity.IsAdmin;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RightAuthorizeAttribute : AuthorizeAttribute
    {
        private string _right;

        public RightAuthorizeAttribute(string right = null)
        {
            _right = right;
        }

        public override bool IsValidForRequest(Controller controller, MethodInfo methodInfo)
        {
            if (controller.User == null)
                return false;
            return controller.User.HasRight(_right ?? controller.Application.Action);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HttpAjaxAttribute : ActionMethodSelectorAttribute
    {
        private bool _value;

        public HttpAjaxAttribute(bool value = true)
        {
            _value = value;
        }

        public override bool IsValidForRequest(Controller controller, MethodInfo methodInfo)
        {
            return controller.IsAjax == _value;
        }
    }
}
