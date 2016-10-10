using System;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace Cnaws.Web
{
    public class HttpModule : IHttpModule
    {
        public void Dispose()
        {
            OnDispose();
        }
        protected virtual void OnDispose()
        {

        }

        public void Init(HttpApplication context)
        {
            OnInit(context);
            context.Error += OnError;
        }
        protected virtual void OnInit(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }
        
        private static bool RedirectToWap(HttpContext context, bool isWap, string wap)
        {
            if (wap != null && wap.Length > 0 && !isWap)
                return MobileUtility.IsMobileRequest(context.Request.ServerVariables["HTTP_USER_AGENT"]);
            return false;
        }

        private void RenderResource(Application app, UrlParse parse, Uri url)
        {
            if (parse.IsDirectory)
            {
                if (url != null && url.Query.StartsWith("??"))
                {
                    try
                    {
                        string path;
                        ResourceHandler handler = null;
                        string[] array = url.Query.Substring(2).Split(',');
                        string[] paths = new string[array.Length];
                        for (int i = 0; i < array.Length; ++i)
                        {
                            path = array[i];
                            if (i == 0)
                            {
                                UrlParse p = new UrlParse(path);
                                handler = ResourceHandler.Parse(p.ExtensionType, p.Extension, false);
                                if (handler == null)
                                    throw new Exception();
                            }
                            paths[i] = app.Context.Server.MapPath(string.Concat(parse.Url, path));
                        }
                        handler.ProcessRequest(app.Context, paths);
                    }
                    catch (Exception)
                    {
                        app.RenderError(404);
                    }
                }
                else
                {
                    app.RenderError(404);
                }
            }
            else
            {
                ResourceHandler handler = ResourceHandler.Parse(parse.ExtensionType, parse.Extension, false);
                if (handler != null && File.Exists(app.Context.Request.PhysicalPath))
                    handler.ProcessRequest(app.Context, app.Context.Request.PhysicalPath);
                else
                    app.RenderError(404);
            }
        }
        private void RenderResource(Application app, UrlParse parse, ExtensionType type, Uri url)
        {
            if (parse.ExtensionType == type)
                RenderResource(app, parse, url);
            else
                app.RenderError(404);
        }
        //private void RenderResource(Application app, UrlParse parse, params ExtensionType[] types)
        //{
        //    if (Array.BinarySearch<ExtensionType>(types, parse.ExtensionType) >= 0)
        //        RenderResource(app, parse);
        //    else
        //        app.RenderError(404);
        //}

        private void RenderRewrite(Application app, UrlParse parse, Uri url)
        {
            if (parse.IsDirectory)
            {
                if (parse.Segments.Count == 0 && parse.Extension == null)
                {
                    app.Run(parse);
                }
                else
                {
                    if (parse.Segments.Count > 0 && parse.Extension == null)
                    {
                        switch (parse.SegmentType)
                        {
                            case SegmentType.Themes:
                                RenderResource(app, parse, url);
                                break;
                            case SegmentType.Resource:
                                app.Run(parse);
                                break;
                            default:
                                app.RenderError(404);
                                break;
                        }
                    }
                    else
                    {
                        app.RenderError(404);
                    }
                }
            }
            else
            {
                if (parse.Segments.Count > 0)
                {
                    if (parse.Extension != null)
                    {
                        switch (parse.SegmentType)
                        {
                            case SegmentType.Resource:
                                parse.AppendExtension();
                                app.Run(parse);
                                break;
                            case SegmentType.Themes:
                            case SegmentType.Uploads:
                                RenderResource(app, parse, url);
                                break;
                            case SegmentType.Favicon:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Ico, null);
                                break;
                            case SegmentType.Robots:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Txt, null);
                                break;
                            case SegmentType.Sitemap:
                            case SegmentType.CrossDomain:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Xml, null);
                                break;
                            case SegmentType.Caches:
                            case SegmentType.Runtime:
                            case SegmentType.Management:
                                app.RenderError(404);
                                break;
                            case SegmentType.Admin:
                                if (parse.ExtensionType != ExtensionType.Html)
                                    parse.AppendExtension();
                                parse.FormatManagement();
                                app.Run(parse);
                                break;
                            case SegmentType.Install:
                                if (parse.ExtensionType != ExtensionType.Html)
                                    parse.AppendExtension();
                                app.Run(parse);
                                break;
                            default:
                                if (parse.ExtensionType == ExtensionType.Html)
                                {
                                    app.Run(parse);
                                }
                                else
                                {
                                    if (parse.Segments.Count > 3 && Utility.StaticString.Equals(parse.Segments[1], StringComparison.OrdinalIgnoreCase))
                                    {
                                        parse.AppendExtension();
                                        app.Run(parse);
                                    }
                                    else
                                    {
                                        app.RenderError(404);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (parse.SegmentType == SegmentType.Admin && parse.Segments.Count == 1)
                        {
                            parse.FormatManagement();
                            app.Run(parse);
                        }
                        else
                        {
                            app.RenderError(404);
                        }
                    }
                }
                else
                {
                    app.RenderError(404);
                }
            }
        }
        private void RenderStatic(Application app, UrlParse parse, Uri url)
        {
            if (parse.IsDirectory)
            {
                if (parse.Segments.Count == 0 && parse.Extension == null)
                {
                    ResourceHandler.Html.ProcessRequest(app.Context, app.Context.Server.MapPath(string.Concat(app.IsWap ? Utility.StaticWapDirF : Utility.StaticWwwDirF, Utility.DefaultController, Utility.DefaultExt)));
                }
                else
                {
                    if (parse.Segments.Count > 0 && parse.Extension == null)
                    {
                        switch (parse.SegmentType)
                        {
                            case SegmentType.Themes:
                                RenderResource(app, parse, url);
                                break;
                            case SegmentType.Resource:
                                app.Run(parse);
                                break;
                            default:
                                app.RenderError(404);
                                break;
                        }
                    }
                    else
                    {
                        app.RenderError(404);
                    }
                }
            }
            else
            {
                if (parse.Segments.Count > 0)
                {
                    if (parse.Extension != null)
                    {
                        switch (parse.SegmentType)
                        {
                            case SegmentType.Resource:
                                parse.AppendExtension();
                                app.Run(parse);
                                break;
                            case SegmentType.Themes:
                            case SegmentType.Uploads:
                                RenderResource(app, parse, url);
                                break;
                            case SegmentType.Favicon:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Ico, null);
                                break;
                            case SegmentType.Robots:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Txt, null);
                                break;
                            case SegmentType.Sitemap:
                            case SegmentType.CrossDomain:
                                if (parse.Segments.Count == 1)
                                    RenderResource(app, parse, ExtensionType.Xml, null);
                                break;
                            case SegmentType.Caches:
                            case SegmentType.Runtime:
                            case SegmentType.Management:
                                app.RenderError(404);
                                break;
                            case SegmentType.Admin:
                                if (parse.ExtensionType != ExtensionType.Html)
                                    parse.AppendExtension();
                                parse.FormatManagement();
                                app.Run(parse);
                                break;
                            case SegmentType.Install:
                                if (parse.ExtensionType != ExtensionType.Html)
                                    parse.AppendExtension();
                                app.Run(parse);
                                break;
                            default:
                                if (parse.ExtensionType == ExtensionType.Html)
                                {
                                    ResourceHandler.Html.ProcessRequest(app.Context, app.Context.Server.MapPath(string.Concat(app.IsWap ? Utility.StaticWapDir : Utility.StaticWwwDir, parse.Url)));
                                }
                                else
                                {
                                    if (parse.Segments.Count > 3 && Utility.StaticString.Equals(parse.Segments[1], StringComparison.OrdinalIgnoreCase))
                                    {
                                        parse.AppendExtension();
                                        app.Run(parse);
                                    }
                                    else
                                    {
                                        app.RenderError(404);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (parse.SegmentType == SegmentType.Admin && parse.Segments.Count == 1)
                        {
                            parse.FormatManagement();
                            app.Run(parse);
                        }
                        else
                        {
                            app.RenderError(404);
                        }
                    }
                }
                else
                {
                    app.RenderError(404);
                }
            }
        }
        private void OnBeginRequest(object sender, EventArgs e)

        {
            HttpContext context = ((HttpApplication)sender).Context;
            Uri uri = context.Request.Url;
            string wap = Settings.Instance.WapDomain;
            string url = context.Request.AppRelativeCurrentExecutionFilePath.Substring(1);
            bool isWap = UrlParse.EqualsDomain(uri.DnsSafeHost, wap);
            if (!RedirectToWap(context, isWap, wap) || "POST".Equals(context.Request.HttpMethod))
            {
                UrlParse parse = new UrlParse(url);
                Application app = new Application(context, isWap);
                if (Settings.Instance.UrlMode == Configuration.SiteUrlMode.Static)
                    RenderStatic(app, parse, uri);
                else
                    RenderRewrite(app, parse, uri);
            }
            else
            {
                url = string.Concat(uri.Scheme, "://", wap, uri.Port != 80 ? string.Concat(":", uri.Port.ToString()) : string.Empty, url);
                try { context.Response.Redirect(url, true); }
                catch { }
            }
        }

        private void OnError(object sender, EventArgs e)
        {
            Application.Current.RenderError();
        }
    }
}
