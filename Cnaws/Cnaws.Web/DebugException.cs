using System;
using System.Web;

namespace Cnaws.Web
{
    public class DebugException : Exception
    {
        public DebugException(string msg)
            : base(msg)
        {
        }
    }
    public class JsonResultException : Exception
    {
        private int _code;

        public JsonResultException(int code)
        {
            _code = code;
        }

        public int Code
        {
            get { return _code; }
        }
    }
}
