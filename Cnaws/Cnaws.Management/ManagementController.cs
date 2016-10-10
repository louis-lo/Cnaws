using System;
using System.IO;
using System.Reflection;
using Cnaws.Web;
using C = Cnaws.Management.Controllers;

namespace Cnaws.Management
{
    public abstract class ManagementController : ResourceDataController
    {
        private C.Management _management;

        public ManagementController()
        {
            _management = null;
        }
        internal void SetManagement(C.Management management)
        {
            _management = management;
            InitController(_management);
            this["this"] = this;
        }

        protected bool CheckAjax()
        {
            return _management.CheckAjax();
        }
        protected bool CheckRight()
        {
            return _management.CheckRight();
        }
        protected bool CheckPost(string key, Action action = null)
        {
            return _management.CheckPost(Namespace, key, action, GetType(), this);
        }
        protected void WriteLog(string msg)
        {
            _management.WriteLog(msg);
        }
        protected void WritePostLog(string name)
        {
            _management.WritePostLog(name);
        }
        protected internal override void SetResult(int code, object value = null)
        {
            _management.SetResult(code, value);
        }
        protected internal override void SetResult(bool result, object value = null)
        {
            _management.SetResult(result, value);
        }
        protected internal override void SetResult(object value)
        {
            _management.SetResult(value);
        }
        protected internal override void SetResult(DataStatus status, Action action, object value = null)
        {
            _management.SetResult(status, action, value);
        }
        protected internal override void SetResult(bool result, Action action, object value = null)
        {
            _management.SetResult(result, action, value);
        }
        protected internal override void SetResult(Action action)
        {
            _management.SetResult(action);
        }
        protected internal override void SetResult<T>(T module, Action<T> action)
        {
            _management.SetResult<T>(module, action);
        }
        protected internal override void SetJavascript(string name, object data)
        {
            _management.SetJavascript(name, data);
        }
        protected internal override void SetJsonp(object data)
        {
            _management.SetJsonp(data);
        }
        protected internal override void NotFound()
        {
            _management.NotFound();
        }
        protected internal override void Unauthorized(bool redirect = false)
        {
            _management.Unauthorized(redirect);
        }
    }
}
