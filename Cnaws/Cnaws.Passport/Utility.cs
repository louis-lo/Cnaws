using Cnaws.Data;
using Cnaws.Passport.Modules;
using Cnaws.Web;
using Cnaws.Web.Configuration;
using System;
using System.Web;

namespace Cnaws.Passport
{
    internal static class Utility
    {
        private const string ReferenceCookieName = "CNAWS.PASSPORT.REFERENCE";

        public static string GetReferenceUrl(PassportController ctl)
        {
            if (ctl.User != null && ctl.User.Identity != null && ctl.User.Identity.IsAuthenticated && !ctl.User.Identity.IsAdmin)
                return ctl.GetUrl(string.Concat(ctl.GetAutoPassportUrl(), "/reference/index/", ctl.User.Identity.Id));
            return string.Empty;
        }
        public static void SetReference(Controller ctl, long userId)
        {
            ctl.Request.Cookies[ReferenceCookieName].Value = userId.ToString();
            ctl.Request.Cookies[ReferenceCookieName].Domain = PassportSection.GetSection().CookieDomain;
        }
        public static long GetReference(Controller ctl, DataSource ds)
        {
            HttpCookie cookie = ctl.Request.Cookies[ReferenceCookieName];
            if (cookie != null)
            {
                long userId;
                if (long.TryParse(cookie.Value, out userId))
                {
                    if (Member.HasId(ds, userId))
                        return userId;
                }
            }
            return 0L;
        }
    }
}
