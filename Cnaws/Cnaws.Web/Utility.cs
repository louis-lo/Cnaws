using System;

namespace Cnaws.Web
{
    public static class Utility
    {
        //public const string UrlQueryName = "_url";
        //public const string RewritePage = "/Default.aspx";
        //public const string RewriteUrl = "~/Default.aspx?_url=";
        //public const string RewriteUrlPath = "Default.aspx?_url=";
        
        internal const string DefaultController = "index";
        internal const string DefaultAction = "index";
        internal const string DefaultTheme = "default";
        internal const string NormalAction = "default";
        internal const string DefaultExt = ".html";
        internal const string DefaultManagement = "management";
        internal const string StaticString = "static";
        
        internal const string ControllerTypeCacheName = "$CtlTps";
        internal const string ModuleTypeCacheName = "$MdlTps";
        internal const string ActionTypeCacheName = "$ActTps";
        internal const string ApplicationItemName = "$App";
        internal const string SubDomainItemName = "$SubDomain";
        internal const string IsWapItemName = "$IsWap";
        internal const string PassportCacheName = "passport";
        internal const string PassportAdminCacheName = "admin";
        internal const string PassportUserCacheName = "user";

        internal const string WwwDir = "www";
        internal const string WapDir = "wap";
        public const string UploadDir = "uploads/";
        internal const string CacheDir = "~/runtime/caches/";
        internal const string StaticWwwDir = "~/runtime/www";
        internal const string StaticWwwDirF = "~/runtime/www/";
        internal const string StaticWapDir = "~/runtime/wap";
        internal const string StaticWapDirF = "~/runtime/wap/";
        internal const string ConfigDir = "~/runtime/config/";
        internal const string ConfigDirP = "/runtime/config/";
        
        internal const string CaptchaChars = "2345678ABCDEFGHJKLMNPQRSTUVWXYZabcdefhijkmnpqrstuvwxyz";
        internal const string CaptchaCookieName = "CNAWS.CAPTCHA.";
        internal const int CaptchaDefaultWidth = 20;//单个字体的宽度范围
        internal const int CaptchaDefaultHeight = 40;//单个字体的高度范围
        internal const int CaptchaDefaultCount = 4;//验证码位数
        internal const int CaptchaExpiration = 30;

        internal const string SMSCaptchaChars = "0123456789";
        internal const int SMSCaptchaDefaultCount = 6;//验证码位数
        internal const int SMSCaptchaTimeSpan = 60;
        internal const int SMSCaptchaExpiration = 300;

        internal const string PassportCookieName = "CNAWS.AUTH";
        internal const string PassportCookieIV = "zwcr8cai";
        internal const string PassportCookieKey = "wtl0tnw3rri9s7gqdpfi0sqm";
        public const string PassportNameRegularExpression = "^[a-zA-Z]\\w{3,31}$";
        public const string PassportPasswordRegularExpression = "^.{6,32}$";
        internal const int PassportMaxInvalidPasswordAttempts = 5;
        internal const int PassportPasswordAnswerAttemptLockoutDuration = 30;

        public const string EmailRegularExpression = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        //public const string MobileRegularExpression = @"^1\d{10}$";
    }
}
