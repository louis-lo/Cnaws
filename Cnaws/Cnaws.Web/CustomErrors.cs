using System;
using System.Web;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using Cnaws.Web.Templates;
using System.Net;
using System.Net.Sockets;

namespace Cnaws.Web
{
    public static class CustomErrors
    {
        private static readonly CustomErrorsMode _mode;
        private static readonly CustomErrorsRedirectMode _redirectMode;
        private static readonly string _defaultRedirect;
        private static readonly Hashtable _errors;
        private static readonly IPAddress[] _ips;

        static CustomErrors()
        {
            _errors = new Hashtable();
            CustomErrorsSection section = (CustomErrorsSection)WebConfigurationManager.GetSection("system.web/customErrors");
            _mode = section.Mode;
            _redirectMode = section.RedirectMode;
            _defaultRedirect = section.DefaultRedirect;
            foreach (CustomError e in section.Errors)
            {
                if (!string.IsNullOrEmpty(e.Redirect))
                    _errors.Add(e.StatusCode, e.Redirect);
            }
            _ips = Dns.GetHostAddresses("localhost");
        }

        internal static bool IsCustom(Application app)
        {
            if (_mode == CustomErrorsMode.On || (_mode == CustomErrorsMode.RemoteOnly && !IsLocation(app.Context.Request.UserHostAddress)))
                return true;
            return false;
        }
        private static bool IsLocation(string ip)
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(ip);
                foreach (IPAddress item in _ips)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork
                        || item.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        if (item.Equals(ipa))
                            return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        private static string FormatMessage(string s)
        {
            return s.Replace("\r\n", "<br/>");
        }
        internal static void ThrowException(Application app, HttpException ex)
        {
            int status = ex.GetHttpCode();
            app.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            app.Context.Response.StatusCode = status;
            Exception e = ex.GetBaseException();
            string msg;
            if (e is TemplateException)
            {
                msg = FormatMessage(e.Message);
            }
            else
            {
                msg = string.Concat(@"<!DOCTYPE html>
<html>
<head>
<title>", status.ToString(), " ", HttpWorkerRequest.GetStatusDescription(status), @"</title>
</head>
<body>", FormatMessage(string.Concat(e.Message, "\r\n", e.StackTrace)), @"</body>
</html>");
            }
            app.Context.Response.Write(msg);
            app.Context.Response.End();
        }
        internal static void RenderCustomError(Application app, int status)
        {
            string url = _errors[status] as string;
            if (url == null)
                url = _defaultRedirect;
            if (_redirectMode == CustomErrorsRedirectMode.ResponseRedirect)
            {
                try { app.Context.Response.Redirect(url, true); }
                catch (Exception) { }
            }
            else
            {
                Application.Current.Run(url.TrimStart('~'));
            }
            app.Context.Response.End();
        }
    }
}
