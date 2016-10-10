using System;

namespace Cnaws.Passport.OAuth2
{
    public class OAuth2Exception : Exception
    {
        private int _code;

        public OAuth2Exception(int code, string msg)
            : base(msg)
        {
            _code = code;
        }

        public int Code
        {
            get { return _code; }
        }
    }
}
