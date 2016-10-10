using System;
using System.IO;
using System.Web;
using System.Threading;
using System.Reflection;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Web
{
    public abstract class ResourceController : Controller
    {
        protected abstract Version Version { get; }
        protected abstract string Namespace { get; }

        internal static string FormatName(string ns, string name)
        {
            return string.Concat(ns, '.', name.ToLower());
        }
        protected string FormatName(string name)
        {
            return string.Concat(Namespace, name);
        }

        private static void RenderResource(string name, string contentType, Application app, Type type, Version version)
        {
            try
            {
                Assembly asm = Assembly.GetAssembly(type);
                using (Stream s = asm.GetManifestResourceStream(name))
                {
                    app.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    app.Context.Response.Cache.SetMaxAge(DateTime.MaxValue - DateTime.Now);
                    string ticks = version.ToString(4);
                    app.Context.Response.Cache.SetETag(ticks);
                    bool hascache = false;
                    string etag = app.Context.Request.Headers["If-None-Match"];
                    if (!string.IsNullOrEmpty(etag))
                        hascache = string.Equals(ticks, etag);
                    if (hascache)
                    {
                        app.Context.Response.StatusCode = 304;
                    }
                    else
                    {
                        app.Context.Response.StatusCode = 200;
                        app.Context.Response.ContentType = contentType;
                        app.Context.Response.Cache.SetLastModified(DateTime.Now);
                        int n;
                        long read = 0;
                        long count = s.Length;
                        byte[] buff = new byte[4096];
                        while (count > 0)
                        {
                            n = s.Read(buff, 0, 4096);
                            if (n == 0)
                                break;
                            if (n == 4096)
                            {
                                app.Context.Response.BinaryWrite(buff);
                            }
                            else
                            {
                                byte[] temp = new byte[n];
                                Array.Copy(buff, temp, n);
                                app.Context.Response.BinaryWrite(temp);
                            }
                            read += n;
                            count -= n;
                        }
                    }
                    app.Context.Response.End();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception)
            {
                app.RenderError(404);
            }
        }
        private static void RenderResource(string[] names, string contentType, Application app, Type type, Version version)
        {
            try
            {
                app.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
                app.Context.Response.Cache.SetMaxAge(DateTime.MaxValue - DateTime.Now);
                string ticks = version.ToString(4);
                app.Context.Response.Cache.SetETag(ticks);
                bool hascache = false;
                string etag = app.Context.Request.Headers["If-None-Match"];
                if (!string.IsNullOrEmpty(etag))
                    hascache = string.Equals(ticks, etag);
                if (hascache)
                {
                    app.Context.Response.StatusCode = 304;
                }
                else
                {
                    app.Context.Response.StatusCode = 200;
                    app.Context.Response.ContentType = contentType;
                    app.Context.Response.Cache.SetLastModified(DateTime.Now);
                    int n;
                    long count;
                    byte[] temp;
                    long read = 0;
                    byte[] buff = new byte[4096];
                    Assembly asm = Assembly.GetAssembly(type);
                    foreach (string name in names)
                    {
                        using (Stream s = asm.GetManifestResourceStream(name))
                        {
                            count = s.Length;
                            while (count > 0)
                            {
                                n = s.Read(buff, 0, 4096);
                                if (n == 0)
                                    break;
                                if (n == 4096)
                                {
                                    app.Context.Response.BinaryWrite(buff);
                                }
                                else
                                {
                                    temp = new byte[n];
                                    Array.Copy(buff, temp, n);
                                    app.Context.Response.BinaryWrite(temp);
                                }
                                read += n;
                                count -= n;
                            }
                        }
                    }
                }
                app.Context.Response.End();
            }
            catch (ThreadAbortException) { }
            catch (Exception)
            {
                app.RenderError(404);
            }
        }
        internal static void RenderResource(string ns, string name, ExtensionType extType, string ext, Arguments args, Application app, Type type, Version version, bool html)
        {
            for (int i = 0; i < args.Count; ++i)
                name = string.Concat(name, '.', args[i]);
            string query = app.Context.Request.Url.Query;
            if (!string.IsNullOrEmpty(query) && query.StartsWith("??"))
            {
                if (query.Length > 2)
                {
                    string path;
                    ResourceHandler handler = null;
                    string[] array = query.Substring(2).Split(',');
                    string[] names = new string[array.Length];
                    for (int i = 0; i < array.Length; ++i)
                    {
                        path = array[i];
                        if (i == 0)
                        {
                            UrlParse p = new UrlParse(path);
                            handler = ResourceHandler.Parse(p.ExtensionType, p.Extension, false);
                            if (handler == null)
                            {
                                app.RenderError(404);
                                return;
                            }
                        }
                        names[i] = FormatName(ns, string.Concat(name, '.', path));
                    }
                    RenderResource(names, handler.ContentType, app, type, version);
                }
                else
                {
                    app.RenderError(404);
                }
            }
            else
            {
                ResourceHandler hander = ResourceHandler.Parse(extType, ext, html);
                if (hander != null)
                    RenderResource(FormatName(ns, name), hander.ContentType, app, type, version);
                else
                    app.RenderError(404);
            }
        }
        protected void RenderResource(string name, Arguments args, bool html)
        {
            RenderResource(Namespace, name, ExtensionType, Extension, args, Application, GetType(), Version, html);
        }

        internal static void RenderTemplate(string ns, string path, Type type, IRenderer renderer)
        {
            string name = string.Concat("html.", path);
            Assembly asm = Assembly.GetAssembly(type);
            using (Stream s = asm.GetManifestResourceStream(FormatName(ns, name)))
            {
                using (StreamReader reader = new StreamReader(s))
                    renderer.Render(reader.ReadToEnd(), path);
            }
        }
        protected void RenderTemplate(string path)
        {
            RenderTemplate(Namespace, path, GetType(), this);
        }
    }

    public abstract class ResourceDataController : ResourceController, IDataController
    {
        private DataSource _ds;

        public ResourceDataController()
        {
            _ds = null;
        }

        public virtual string DataProvider
        {
            get { return Application.Settings.DataProvider; }
        }

        public DataSource DataSource
        {
            get
            {
                if (_ds == null)
                    _ds = new DataSource(DataProvider);
                return _ds;
            }
        }

        public void InitController(ResourceDataController controller)
        {
            base.InitController(controller);
            if (controller._ds != null)
                _ds = new DataSource(controller._ds);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                    _ds = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
