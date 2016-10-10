using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using Cnaws.Web.Templates;
using Cnaws.Web.Configuration;

namespace Cnaws.Web
{
    public sealed class Settings
    {
        private static readonly Settings _configSettings;

        //private string[] _controllerNamespaces;
        private string _rootUrl;
        private string _theme;
        private string _themePath;
        private string _themeUrl;
        private bool _debug;
        private Encoding _responseEncoding;
        private Encoding _fileEncoding;
        private SiteUrlMode _urlMode;
        //private string _urlExt;
        private string _management;
        private string _wapDomain;
        private string _subDomain;
        private string _dataProvider;
        private CacheMode _cacheMode;
        private string _cacheProvider;
        private string _resources;
        private string _passport;
        private string _wapPassport;

        static Settings()
        {
            //PagesSection ps = (PagesSection)WebConfigurationManager.GetSection("system.web/pages");
            //List<string> ns = new List<string>(ps.Namespaces.Count);
            //foreach (NamespaceInfo n in ps.Namespaces)
            //    ns.Add(n.Namespace);
            CompilationSection cs = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation");
            GlobalizationSection gs = (GlobalizationSection)WebConfigurationManager.GetSection("system.web/globalization");
            SiteSection ss = SiteSection.GetSection();
            _configSettings = new Settings();
            _configSettings._rootUrl = HttpContext.Current.Request.ApplicationPath;
            if (!_configSettings._rootUrl.EndsWith("/"))
                _configSettings._rootUrl = string.Concat(_configSettings._rootUrl, "/");
            //_configSettings._controllerNamespaces = ns.ToArray();
            _configSettings._theme = ss.Theme;
            _configSettings._themePath = GetTempPath(_configSettings._theme);
            _configSettings._themeUrl = string.Concat(_configSettings._rootUrl, "themes/", _configSettings._theme, "/");
            _configSettings._debug = cs.Debug;
            _configSettings._responseEncoding = gs.ResponseEncoding;
            _configSettings._fileEncoding = gs.FileEncoding;
            _configSettings._urlMode = ss.UrlMode;
            //_configSettings._urlExt = ss.UrlExt;
            _configSettings._management = ss.Management;
            _configSettings._wapDomain = ss.WapDomain;
            _configSettings._subDomain = ss.SubDomain;
            _configSettings._dataProvider = ss.DataProvider;
            _configSettings._cacheMode = ss.CacheMode;
            _configSettings._cacheProvider = ss.CacheProvider;
            _configSettings._resources = ss.ResourcesUrl;
            _configSettings._passport = ss.PassportUrl;
            _configSettings._wapPassport = ss.WapPassportUrl;
        }

        private Settings()
        {
            //_controllerNamespaces = new string[] { "Cnaws.Web.Controllers" };
            //_theme = Utility.DefaultTheme;
            //_themePath = GetTempPath(_theme);
            //_themeUrl = "";
            //_debug = false;
            //_responseEncoding = Encoding.UTF8;
            //_fileEncoding = Encoding.UTF8;
        }

        internal static Settings Instance
        {
            get { return _configSettings; }
        }

        private static string GetTempPath(string theme)
        {
            string path = HttpContext.Current.Server.MapPath(string.Concat("~/themes/", theme));
            Resources.Paths.Clear();
            Resources.Paths.Add(path);
            return path;
        }

        public string[] GetThemes()
        {
            string dir = HttpContext.Current.Server.MapPath(string.Concat("~/themes"));
            DirectoryInfo di = new DirectoryInfo(dir);
            return Array.ConvertAll<DirectoryInfo, string>(di.GetDirectories(), new Converter<DirectoryInfo, string>((x) => { return x.Name; }));
        }

        //public string[] ControllerNamespaces
        //{
        //    get { return _controllerNamespaces; }
        //}

        public string RootUrl
        {
            get { return _rootUrl; }
        }

        public string Theme
        {
            get { return _theme; }
        }

        public string ThemePath
        {
            get { return _themePath; }
        }

        public string ThemeUrl
        {
            get { return _themeUrl; }
        }

        public bool Debug
        {
            get { return _debug; }
        }

        public Encoding ResponseEncoding
        {
            get { return _responseEncoding; }
        }

        public Encoding FileEncoding
        {
            get { return _fileEncoding; }
        }

        public SiteUrlMode UrlMode
        {
            get { return _urlMode; }
        }

        //public string UrlExt
        //{
        //    get { return _urlExt; }
        //}

        public string Management
        {
            get { return _management; }
        }

        public string WapDomain
        {
            get { return _wapDomain; }
        }

        public string SubDomain
        {
            get { return _subDomain; }
        }

        public string DataProvider
        {
            get { return _dataProvider; }
        }

        public CacheMode CacheMode
        {
            get { return _cacheMode; }
        }

        public string CacheProvider
        {
            get { return _cacheProvider; }
        }

        public string ResourcesUrl
        {
            get { return _resources; }
        }

        public string PassportUrl
        {
            get { return _passport; }
        }
        public string WapPassportUrl
        {
            get { return _wapPassport; }
        }
    }
}
