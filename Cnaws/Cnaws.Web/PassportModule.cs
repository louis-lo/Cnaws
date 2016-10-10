using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Security.Principal;
using System.Collections.Generic;
using Cnaws.Security;
using Cnaws.Templates;
using Cnaws.Configuration;
using Cnaws.Web.Configuration;

namespace Cnaws.Web
{
    public interface IPassportUserInfo
    {
        long Id { get; }
        long AdminId { get; }
        string Name { get; }
        long RoleId { get; }
        long AdminRoleId { get; }
        DateTime CreationDate { get; }
        string LastIp { get; }
        DateTime LastTime { get; }
        long LoginCount { get; }
        string UserData { get; }
    }

    public class PassportIdentity : IIdentity
    {
        private bool _userInited;
        private bool _isAuthenticated;
        private bool _isAdmin;
        private long _id;
        private long _adminId;
        private string _name;
        private long _roleId;
        private long _adminRoleId;
        private DateTime _creationDate;
        private string _lastIp;
        private DateTime _lastTime;
        private long _loginCount;
        private string _userData;
        private string _sysData;

        internal PassportIdentity()
        {
            _userInited = false;
        }
        internal PassportIdentity(string token)
        {
            _userInited = true;
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new Exception();
                byte[] bytes = PassportAuthentication.DecodeCookie(token);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        _isAuthenticated = reader.ReadByte() == 1;
                        _isAdmin = reader.ReadByte() == 1;
                        _id = reader.ReadInt64();
                        _adminId = reader.ReadInt64();
                        _name = string.Empty;
                        _roleId = reader.ReadInt64();
                        _adminRoleId = reader.ReadInt64();
                        _creationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                        _lastIp = IPAddress.Any.ToString();
                        _lastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                        _loginCount = 0L;
                        _userData = string.Empty;
                        _sysData = string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                _isAuthenticated = false;
                _isAdmin = false;
                _id = 0L;
                _adminId = 0L;
                _name = string.Empty;
                _roleId = 0L;
                _adminRoleId = 0L;
                _creationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                _lastIp = IPAddress.Any.ToString();
                _lastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                _loginCount = 0L;
                _userData = string.Empty;
                _sysData = string.Empty;
            }
        }
        internal PassportIdentity(bool isAuthenticated, bool isAdmin, IPassportUserInfo user)
        {
            _isAuthenticated = isAuthenticated;
            _isAdmin = isAdmin;
            _id = user.Id;
            _adminId = user.AdminId;
            _name = user.Name;
            _roleId = user.RoleId;
            _adminRoleId = user.AdminRoleId;
            _creationDate = user.CreationDate;
            _lastIp = user.LastIp;
            _lastTime = user.LastTime;
            _loginCount = user.LoginCount;
            _userData = user.UserData;
            switch (PassportAuthentication.Level)
            {
                case PassportLevel.Normal:
                    {
                        _sysData = Controller.GetClientIp();
                    }
                    break;
                case PassportLevel.High:
                    {
                        string id = Guid.NewGuid().ToString("N");
                        CacheProvider.Current.Set(new string[] { Utility.PassportCacheName, _isAdmin ? Utility.PassportAdminCacheName : Utility.PassportUserCacheName, _id.ToString() }, id);
                        _sysData = id;
                    }
                    break;
                default:
                    {
                        _sysData = string.Empty;
                    }
                    break;
            }
            _userInited = true;
        }

        private void EnsureUserInfo()
        {
            if (!_userInited)
            {
                try
                {
                    HttpContext context = HttpContext.Current;
                    HttpCookie cookie = context.Request.Cookies[PassportAuthentication.CookieName];
                    if (cookie == null)
                        throw new Exception();
                    string value = cookie.Value;
                    if (string.IsNullOrEmpty(value))
                        throw new Exception();
                    byte[] bytes = PassportAuthentication.DecodeCookie(value);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        using (BinaryReader reader = new BinaryReader(ms))
                        {
                            _isAuthenticated = reader.ReadByte() == 1;
                            _isAdmin = reader.ReadByte() == 1;
                            _id = reader.ReadInt64();
                            _adminId = reader.ReadInt64();
                            _name = reader.ReadString();
                            _roleId = reader.ReadInt64();
                            _adminRoleId = reader.ReadInt64();
                            _creationDate = new DateTime(reader.ReadInt64());
                            int len = reader.ReadByte();
                            byte[] buff = reader.ReadBytes(len);
                            _lastIp = (new IPAddress(buff)).ToString();
                            _lastTime = new DateTime(reader.ReadInt64());
                            _loginCount = reader.ReadInt64();
                            _userData = reader.ReadString();
                            _sysData = reader.ReadString();
                            switch (PassportAuthentication.Level)
                            {
                                case PassportLevel.Normal:
                                    if (!string.Equals(_sysData, Controller.GetClientIp(context)))
                                        throw new Exception();
                                    break;
                                case PassportLevel.High:
                                    if (!string.Equals(_sysData, CacheProvider.Current.Get<string>(new string[] { Utility.PassportCacheName, _isAdmin ? Utility.PassportAdminCacheName : Utility.PassportUserCacheName, _id.ToString() })))
                                        throw new Exception();
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    _isAuthenticated = false;
                    _isAdmin = false;
                    _id = 0L;
                    _adminId = 0L;
                    _name = string.Empty;
                    _roleId = 0L;
                    _adminRoleId = 0L;
                    _creationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                    _lastIp = IPAddress.Any.ToString();
                    _lastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
                    _loginCount = 0L;
                    _userData = string.Empty;
                    _sysData = string.Empty;
                }

                _userInited = true;
            }
        }

        public string AuthenticationType
        {
            get { return "Cnaws.Passport"; }
        }

        public bool IsAuthenticated
        {
            get
            {
                EnsureUserInfo();
                return _isAuthenticated;
            }
        }
        public bool IsAdmin
        {
            get
            {
                EnsureUserInfo();
                return _isAdmin;
            }
        }
        public long Id
        {
            get
            {
                EnsureUserInfo();
                return _id;
            }
        }
        public long AdminId
        {
            get
            {
                EnsureUserInfo();
                return _adminId;
            }
        }
        public string Name
        {
            get
            {
                EnsureUserInfo();
                return _name;
            }
        }
        public long RoleId
        {
            get
            {
                EnsureUserInfo();
                return _roleId;
            }
        }
        public long AdminRoleId
        {
            get
            {
                EnsureUserInfo();
                return _adminRoleId;
            }
        }
        public DateTime CreationDate
        {
            get
            {
                EnsureUserInfo();
                return _creationDate;
            }
        }
        public string LastIp
        {
            get
            {
                EnsureUserInfo();
                return _lastIp;
            }
        }
        public DateTime LastTime
        {
            get
            {
                EnsureUserInfo();
                return _lastTime;
            }
        }
        public long LoginCount
        {
            get
            {
                EnsureUserInfo();
                return _loginCount;
            }
        }
        public string UserData
        {
            get
            {
                EnsureUserInfo();
                return _userData;
            }
        }

        public override string ToString()
        {
            IPAddress ip;
            if (!IPAddress.TryParse(_lastIp, out ip))
                ip = IPAddress.Any;
            byte[] buff = ip.GetAddressBytes();
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write((byte)(_isAuthenticated ? 1 : 0));
                    writer.Write((byte)(_isAdmin ? 1 : 0));
                    writer.Write(_id);
                    writer.Write(_adminId);
                    writer.Write(_name);
                    writer.Write(_roleId);
                    writer.Write(_adminRoleId);
                    writer.Write(_creationDate.Ticks);
                    writer.Write((byte)buff.Length);
                    writer.Write(buff);
                    writer.Write(_lastTime.Ticks);
                    writer.Write(_loginCount);
                    writer.Write(_userData);
                    writer.Write(_sysData);
                }
                return PassportAuthentication.EncodeCookie(ms.ToArray());
            }
        }

        public string GetToken()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write((byte)(_isAuthenticated ? 1 : 0));
                    writer.Write((byte)(_isAdmin ? 1 : 0));
                    writer.Write(_id);
                    writer.Write(_adminId);
                    writer.Write(_roleId);
                    writer.Write(_adminRoleId);
                }
                return PassportAuthentication.EncodeCookie(ms.ToArray());
            }
        }
    }

    public class PassportPrincipal : GenericPrincipal
    {
        public PassportPrincipal(PassportIdentity identity)
            : base(identity, null)
        {
        }

        public new PassportIdentity Identity
        {
            get { return (PassportIdentity)base.Identity; }
        }

        public override bool IsInRole(string role)
        {
            return false;
        }

        public virtual bool HasRight(string right)
        {
            return false;
        }
    }

    public abstract class PassportProvider
    {
        protected PassportProvider()
        {
        }

        internal protected virtual PassportIdentity CreateIdentity(bool isAuthenticated, bool isAdmin, IPassportUserInfo user)
        {
            return new PassportIdentity(isAuthenticated, isAdmin, user);
        }
        internal protected abstract PassportPrincipal CreatePrincipal(PassportIdentity identity);
    }

    public static class PassportAuthentication
    {
        private static readonly string _cookieName;
        private static readonly byte[] _ivBytes;
        private static readonly byte[] _keyBytes;
        private static readonly string _cookieDomain;
        private static readonly int _maxInvalidPasswordAttempts;
        private static readonly int _passwordAnswerAttemptLockoutDuration;
        private static readonly Configuration.PassportLevel _level;
        private static readonly string _dataProvider;

        static PassportAuthentication()
        {
            PassportSection section = PassportSection.GetSection();
            _ivBytes = Encoding.UTF8.GetBytes(section.CookieIV);
            _keyBytes = Encoding.UTF8.GetBytes(section.CookieKey);
            _cookieName = section.CookieName;
            _cookieDomain = section.CookieDomain;
            _maxInvalidPasswordAttempts = section.MaxInvalidPasswordAttempts;
            _passwordAnswerAttemptLockoutDuration = section.PasswordAnswerAttemptLockoutDuration;
            _level = section.Level;
            _dataProvider = section.DataProvider;
        }

        public static string CookieName
        {
            get { return _cookieName; }
        }
        public static int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }
        public static int PasswordAnswerAttemptLockoutDuration
        {
            get { return _passwordAnswerAttemptLockoutDuration; }
        }
        public static Configuration.PassportLevel Level
        {
            get { return _level; }
        }
        public static string DataProvider
        {
            get { return _dataProvider; }
        }

        public static string EncodeCookie(byte[] bytes)
        {
            return CryptoUtility.TripleDESEncrypt(bytes, _keyBytes, _ivBytes);
        }
        public static byte[] DecodeCookie(string s)
        {
            return CryptoUtility.TripleDESDecrypt(s, _keyBytes, _ivBytes);
        }

        public static void SetAuthCookie(bool isAuthenticated, bool isAdmin, IPassportUserInfo user, HttpContext context = null)
        {
            PassportIdentity i = new PassportIdentity(isAuthenticated, isAdmin, user);
            PassportPrincipal p = new PassportPrincipal(i);
            SetAuthCookie(p, context);
        }
        public static void SetAuthCookie<T>(bool isAuthenticated, bool isAdmin, IPassportUserInfo user, HttpContext context = null) where T : PassportProvider, new()
        {
            T p = new T();
            SetAuthCookie(p.CreatePrincipal(p.CreateIdentity(isAuthenticated, isAdmin, user)), context);
        }
        private static void SetAuthCookie(PassportPrincipal p, HttpContext context)
        {
            if (context == null)
                context = HttpContext.Current;
            context.Response.Cookies[CookieName].Value = ((p != null && p.Identity != null) ? p.Identity.ToString() : string.Empty);
            if (!string.IsNullOrEmpty(_cookieDomain)
#if (DEBUG)
                && !"localhost".Equals(_cookieDomain)
#endif
                )
                context.Response.Cookies[CookieName].Domain = _cookieDomain;
            context.User = p;
        }
        public static void SetCustomCookie(string name, byte[] value, HttpContext context = null)
        {
            if (context == null)
                context = HttpContext.Current;
            context.Response.Cookies[name].Value = EncodeCookie(value);
            if (!string.IsNullOrEmpty(_cookieDomain)
#if (DEBUG)
                && !"localhost".Equals(_cookieDomain)
#endif
                )
                context.Response.Cookies[name].Domain = _cookieDomain;
        }
        public static byte[] GetCustomCookie(string name, HttpContext context = null)
        {
            if (context == null)
                context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies[name];
            if (cookie != null)
                return DecodeCookie(cookie.Value);
            return null;
        }
        public static void SetAuthToken(string token, HttpContext context)
        {
            SetAuthCookie(new PassportPrincipal(new PassportIdentity(token)), context);
        }

        public static void SignOut()
        {
            HttpContext context = HttpContext.Current;
            PassportPrincipal pp = context.User as PassportPrincipal;
            if (pp != null)
                CacheProvider.Current.Set(new string[] { Utility.PassportCacheName, pp.Identity.IsAdmin ? Utility.PassportAdminCacheName : Utility.PassportUserCacheName, pp.Identity.Id.ToString() }, null);
            SetAuthCookie(null, context);
        }
    }

    public sealed class PassportModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        private void OnBeginRequest(object source, EventArgs eventArgs)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            context.User = new PassportPrincipal(new PassportIdentity());
        }
    }
}
