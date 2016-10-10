using System;
using System.Web;

namespace Cnaws.Web
{
    public sealed class SubSiteModule : IHttpModule
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
            string sub = null;
            string subDomain = Settings.Instance.SubDomain;
            if (!string.IsNullOrEmpty(subDomain))
                sub = GetSubDomain(context.Request.Url.DnsSafeHost, subDomain);
            context.Items[Utility.SubDomainItemName] = sub;
        }

        internal static unsafe string GetSubDomain(string domain, string subDomain)
        {
            int len = subDomain.Length;
            int num = domain.Length - len;
            if (num > 0)
            {
                fixed (char* str1 = domain)
                {
                    char* p = str1;
                    char* p1 = str1 + num;
                    fixed (char* str2 = subDomain)
                    {
                        char* p2 = str2;
                        for (int i = 0; i < len; ++i)
                        {
                            if (p1[i] != p2[i])
                                return null;
                        }
                    }
                    return new string(p, 0, num);
                }
            }
            return null;
        }
    }
}
