using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Web
{
    internal enum ExtensionType
    {
        Unknown,
        //Css,
        //Js,
        Png,
        //Jpg,
        //Gif,
        Ico,
        Txt,
        Xml,
        //Swf,
        //Htc,
        Aspx,
        Html,
        //Eot,
        //Woff,
        //Ttf,
        //Svg,
    }

    internal sealed class ResourceHandler
    {
        private static readonly Dictionary<string, ResourceHandler> _mappings;

        public static readonly ResourceHandler Html;
        private static readonly ResourceHandler Default;

        private string _type;

        static ResourceHandler()
        {
            _mappings = new Dictionary<string, ResourceHandler>(StringComparer.OrdinalIgnoreCase);

            Html = new ResourceHandler("text/html");
            Default = new ResourceHandler("application/octet-stream");

            Dictionary<string, ResourceHandler> mappings = new Dictionary<string, ResourceHandler>(StringComparer.OrdinalIgnoreCase);
            mappings.Add(Html.ContentType, Html);
            mappings.Add(Default.ContentType, Default);

            AddMapping(mappings, ".323", "text/h323");
            AddMapping(mappings, ".aaf", "application/octet-stream");
            AddMapping(mappings, ".aca", "application/octet-stream");
            AddMapping(mappings, ".accdb", "application/msaccess");
            AddMapping(mappings, ".accde", "application/msaccess");
            AddMapping(mappings, ".accdt", "application/msaccess");
            AddMapping(mappings, ".acx", "application/internet-property-stream");
            AddMapping(mappings, ".afm", "application/octet-stream");
            AddMapping(mappings, ".ai", "application/postscript");
            AddMapping(mappings, ".aif", "audio/x-aiff");
            AddMapping(mappings, ".aifc", "audio/aiff");
            AddMapping(mappings, ".aiff", "audio/aiff");
            AddMapping(mappings, ".application", "application/x-ms-application");
            AddMapping(mappings, ".art", "image/x-jg");
            AddMapping(mappings, ".asd", "application/octet-stream");
            AddMapping(mappings, ".asf", "video/x-ms-asf");
            AddMapping(mappings, ".asi", "application/octet-stream");
            AddMapping(mappings, ".asm", "text/plain");
            AddMapping(mappings, ".asr", "video/x-ms-asf");
            AddMapping(mappings, ".asx", "video/x-ms-asf");
            AddMapping(mappings, ".atom", "application/atom+xml");
            AddMapping(mappings, ".au", "audio/basic");
            AddMapping(mappings, ".avi", "video/x-msvideo");
            AddMapping(mappings, ".axs", "application/olescript");
            AddMapping(mappings, ".bas", "text/plain");
            AddMapping(mappings, ".bcpio", "application/x-bcpio");
            AddMapping(mappings, ".bin", "application/octet-stream");
            AddMapping(mappings, ".bmp", "image/bmp");
            AddMapping(mappings, ".c", "text/plain");
            AddMapping(mappings, ".cab", "application/octet-stream");
            AddMapping(mappings, ".calx", "application/vnd.ms-office.calx");
            AddMapping(mappings, ".cat", "application/vnd.ms-pki.seccat");
            AddMapping(mappings, ".cdf", "application/x-cdf");
            AddMapping(mappings, ".chm", "application/octet-stream");
            AddMapping(mappings, ".class", "application/x-java-applet");
            AddMapping(mappings, ".clp", "application/x-msclip");
            AddMapping(mappings, ".cmx", "image/x-cmx");
            AddMapping(mappings, ".cnf", "text/plain");
            AddMapping(mappings, ".cod", "image/cis-cod");
            AddMapping(mappings, ".cpio", "application/x-cpio");
            AddMapping(mappings, ".cpp", "text/plain");
            AddMapping(mappings, ".crd", "application/x-mscardfile");
            AddMapping(mappings, ".crl", "application/pkix-crl");
            AddMapping(mappings, ".crt", "application/x-x509-ca-cert");
            AddMapping(mappings, ".csh", "application/x-csh");
            AddMapping(mappings, ".css", "text/css");
            AddMapping(mappings, ".csv", "application/octet-stream");
            AddMapping(mappings, ".cur", "application/octet-stream");
            AddMapping(mappings, ".dcr", "application/x-director");
            AddMapping(mappings, ".deploy", "application/octet-stream");
            AddMapping(mappings, ".der", "application/x-x509-ca-cert");
            AddMapping(mappings, ".dib", "image/bmp");
            AddMapping(mappings, ".dir", "application/x-director");
            AddMapping(mappings, ".disco", "text/xml");
            AddMapping(mappings, ".dll", "application/x-msdownload");
            AddMapping(mappings, ".dll.config", "text/xml");
            AddMapping(mappings, ".dlm", "text/dlm");
            AddMapping(mappings, ".doc", "application/msword");
            AddMapping(mappings, ".docm", "application/vnd.ms-word.document.macroEnabled.12");
            AddMapping(mappings, ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            AddMapping(mappings, ".dot", "application/msword");
            AddMapping(mappings, ".dotm", "application/vnd.ms-word.template.macroEnabled.12");
            AddMapping(mappings, ".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
            AddMapping(mappings, ".dsp", "application/octet-stream");
            AddMapping(mappings, ".dtd", "text/xml");
            AddMapping(mappings, ".dvi", "application/x-dvi");
            AddMapping(mappings, ".dwf", "drawing/x-dwf");
            AddMapping(mappings, ".dwp", "application/octet-stream");
            AddMapping(mappings, ".dxr", "application/x-director");
            AddMapping(mappings, ".eml", "message/rfc822");
            AddMapping(mappings, ".emz", "application/octet-stream");
            AddMapping(mappings, ".eot", "application/octet-stream");
            AddMapping(mappings, ".eps", "application/postscript");
            AddMapping(mappings, ".etx", "text/x-setext");
            AddMapping(mappings, ".evy", "application/envoy");
            AddMapping(mappings, ".exe", "application/octet-stream");
            AddMapping(mappings, ".exe.config", "text/xml");
            AddMapping(mappings, ".fdf", "application/vnd.fdf");
            AddMapping(mappings, ".fif", "application/fractals");
            AddMapping(mappings, ".fla", "application/octet-stream");
            AddMapping(mappings, ".flr", "x-world/x-vrml");
            AddMapping(mappings, ".flv", "video/x-flv");
            AddMapping(mappings, ".gif", "image/gif");
            AddMapping(mappings, ".gtar", "application/x-gtar");
            AddMapping(mappings, ".gz", "application/x-gzip");
            AddMapping(mappings, ".h", "text/plain");
            AddMapping(mappings, ".hdf", "application/x-hdf");
            AddMapping(mappings, ".hdml", "text/x-hdml");
            AddMapping(mappings, ".hhc", "application/x-oleobject");
            AddMapping(mappings, ".hhk", "application/octet-stream");
            AddMapping(mappings, ".hhp", "application/octet-stream");
            AddMapping(mappings, ".hlp", "application/winhlp");
            AddMapping(mappings, ".hqx", "application/mac-binhex40");
            AddMapping(mappings, ".hta", "application/hta");
            AddMapping(mappings, ".htc", "text/x-component");
            AddMapping(mappings, ".htm", "text/html");
            AddMapping(mappings, ".html", "text/html");
            AddMapping(mappings, ".htt", "text/webviewhtml");
            AddMapping(mappings, ".hxt", "text/html");
            AddMapping(mappings, ".ico", "image/x-icon");
            AddMapping(mappings, ".ics", "application/octet-stream");
            AddMapping(mappings, ".ief", "image/ief");
            AddMapping(mappings, ".iii", "application/x-iphone");
            AddMapping(mappings, ".inf", "application/octet-stream");
            AddMapping(mappings, ".ins", "application/x-internet-signup");
            AddMapping(mappings, ".isp", "application/x-internet-signup");
            AddMapping(mappings, ".IVF", "video/x-ivf");
            AddMapping(mappings, ".jar", "application/java-archive");
            AddMapping(mappings, ".java", "application/octet-stream");
            AddMapping(mappings, ".jck", "application/liquidmotion");
            AddMapping(mappings, ".jcz", "application/liquidmotion");
            AddMapping(mappings, ".jfif", "image/pjpeg");
            AddMapping(mappings, ".jpb", "application/octet-stream");
            AddMapping(mappings, ".jpe", "image/jpeg");
            AddMapping(mappings, ".jpeg", "image/jpeg");
            AddMapping(mappings, ".jpg", "image/jpeg");
            AddMapping(mappings, ".js", "application/x-javascript");
            AddMapping(mappings, ".jsx", "text/jscript");
            AddMapping(mappings, ".latex", "application/x-latex");
            AddMapping(mappings, ".lit", "application/x-ms-reader");
            AddMapping(mappings, ".lpk", "application/octet-stream");
            AddMapping(mappings, ".lsf", "video/x-la-asf");
            AddMapping(mappings, ".lsx", "video/x-la-asf");
            AddMapping(mappings, ".lzh", "application/octet-stream");
            AddMapping(mappings, ".m13", "application/x-msmediaview");
            AddMapping(mappings, ".m14", "application/x-msmediaview");
            AddMapping(mappings, ".m1v", "video/mpeg");
            AddMapping(mappings, ".m3u", "audio/x-mpegurl");
            AddMapping(mappings, ".man", "application/x-troff-man");
            AddMapping(mappings, ".manifest", "application/x-ms-manifest");
            AddMapping(mappings, ".map", "text/plain");
            AddMapping(mappings, ".mdb", "application/x-msaccess");
            AddMapping(mappings, ".mdp", "application/octet-stream");
            AddMapping(mappings, ".me", "application/x-troff-me");
            AddMapping(mappings, ".mht", "message/rfc822");
            AddMapping(mappings, ".mhtml", "message/rfc822");
            AddMapping(mappings, ".mid", "audio/mid");
            AddMapping(mappings, ".midi", "audio/mid");
            AddMapping(mappings, ".mix", "application/octet-stream");
            AddMapping(mappings, ".mmf", "application/x-smaf");
            AddMapping(mappings, ".mno", "text/xml");
            AddMapping(mappings, ".mny", "application/x-msmoney");
            AddMapping(mappings, ".mov", "video/quicktime");
            AddMapping(mappings, ".movie", "video/x-sgi-movie");
            AddMapping(mappings, ".mp2", "video/mpeg");
            AddMapping(mappings, ".mp3", "audio/mpeg");
            AddMapping(mappings, ".mpa", "video/mpeg");
            AddMapping(mappings, ".mpe", "video/mpeg");
            AddMapping(mappings, ".mpeg", "video/mpeg");
            AddMapping(mappings, ".mpg", "video/mpeg");
            AddMapping(mappings, ".mpp", "application/vnd.ms-project");
            AddMapping(mappings, ".mpv2", "video/mpeg");
            AddMapping(mappings, ".ms", "application/x-troff-ms");
            AddMapping(mappings, ".msi", "application/octet-stream");
            AddMapping(mappings, ".mso", "application/octet-stream");
            AddMapping(mappings, ".mvb", "application/x-msmediaview");
            AddMapping(mappings, ".mvc", "application/x-miva-compiled");
            AddMapping(mappings, ".nc", "application/x-netcdf");
            AddMapping(mappings, ".nsc", "video/x-ms-asf");
            AddMapping(mappings, ".nws", "message/rfc822");
            AddMapping(mappings, ".ocx", "application/octet-stream");
            AddMapping(mappings, ".oda", "application/oda");
            AddMapping(mappings, ".odc", "text/x-ms-odc");
            AddMapping(mappings, ".ods", "application/oleobject");
            AddMapping(mappings, ".one", "application/onenote");
            AddMapping(mappings, ".onea", "application/onenote");
            AddMapping(mappings, ".onetoc", "application/onenote");
            AddMapping(mappings, ".onetoc2", "application/onenote");
            AddMapping(mappings, ".onetmp", "application/onenote");
            AddMapping(mappings, ".onepkg", "application/onenote");
            AddMapping(mappings, ".osdx", "application/opensearchdescription+xml");
            AddMapping(mappings, ".p10", "application/pkcs10");
            AddMapping(mappings, ".p12", "application/x-pkcs12");
            AddMapping(mappings, ".p7b", "application/x-pkcs7-certificates");
            AddMapping(mappings, ".p7c", "application/pkcs7-mime");
            AddMapping(mappings, ".p7m", "application/pkcs7-mime");
            AddMapping(mappings, ".p7r", "application/x-pkcs7-certreqresp");
            AddMapping(mappings, ".p7s", "application/pkcs7-signature");
            AddMapping(mappings, ".pbm", "image/x-portable-bitmap");
            AddMapping(mappings, ".pcx", "application/octet-stream");
            AddMapping(mappings, ".pcz", "application/octet-stream");
            AddMapping(mappings, ".pdf", "application/pdf");
            AddMapping(mappings, ".pfb", "application/octet-stream");
            AddMapping(mappings, ".pfm", "application/octet-stream");
            AddMapping(mappings, ".pfx", "application/x-pkcs12");
            AddMapping(mappings, ".pgm", "image/x-portable-graymap");
            AddMapping(mappings, ".pko", "application/vnd.ms-pki.pko");
            AddMapping(mappings, ".pma", "application/x-perfmon");
            AddMapping(mappings, ".pmc", "application/x-perfmon");
            AddMapping(mappings, ".pml", "application/x-perfmon");
            AddMapping(mappings, ".pmr", "application/x-perfmon");
            AddMapping(mappings, ".pmw", "application/x-perfmon");
            AddMapping(mappings, ".png", "image/png");
            AddMapping(mappings, ".pnm", "image/x-portable-anymap");
            AddMapping(mappings, ".pnz", "image/png");
            AddMapping(mappings, ".pot", "application/vnd.ms-powerpoint");
            AddMapping(mappings, ".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12");
            AddMapping(mappings, ".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
            AddMapping(mappings, ".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12");
            AddMapping(mappings, ".ppm", "image/x-portable-pixmap");
            AddMapping(mappings, ".pps", "application/vnd.ms-powerpoint");
            AddMapping(mappings, ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12");
            AddMapping(mappings, ".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            AddMapping(mappings, ".ppt", "application/vnd.ms-powerpoint");
            AddMapping(mappings, ".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12");
            AddMapping(mappings, ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            AddMapping(mappings, ".prf", "application/pics-rules");
            AddMapping(mappings, ".prm", "application/octet-stream");
            AddMapping(mappings, ".prx", "application/octet-stream");
            AddMapping(mappings, ".ps", "application/postscript");
            AddMapping(mappings, ".psd", "application/octet-stream");
            AddMapping(mappings, ".psm", "application/octet-stream");
            AddMapping(mappings, ".psp", "application/octet-stream");
            AddMapping(mappings, ".pub", "application/x-mspublisher");
            AddMapping(mappings, ".qt", "video/quicktime");
            AddMapping(mappings, ".qtl", "application/x-quicktimeplayer");
            AddMapping(mappings, ".qxd", "application/octet-stream");
            AddMapping(mappings, ".ra", "audio/x-pn-realaudio");
            AddMapping(mappings, ".ram", "audio/x-pn-realaudio");
            AddMapping(mappings, ".rar", "application/octet-stream");
            AddMapping(mappings, ".ras", "image/x-cmu-raster");
            AddMapping(mappings, ".rf", "image/vnd.rn-realflash");
            AddMapping(mappings, ".rgb", "image/x-rgb");
            AddMapping(mappings, ".rm", "application/vnd.rn-realmedia");
            AddMapping(mappings, ".rmi", "audio/mid");
            AddMapping(mappings, ".roff", "application/x-troff");
            AddMapping(mappings, ".rpm", "audio/x-pn-realaudio-plugin");
            AddMapping(mappings, ".rtf", "application/rtf");
            AddMapping(mappings, ".rtx", "text/richtext");
            AddMapping(mappings, ".scd", "application/x-msschedule");
            AddMapping(mappings, ".sct", "text/scriptlet");
            AddMapping(mappings, ".sea", "application/octet-stream");
            AddMapping(mappings, ".setpay", "application/set-payment-initiation");
            AddMapping(mappings, ".setreg", "application/set-registration-initiation");
            AddMapping(mappings, ".sgml", "text/sgml");
            AddMapping(mappings, ".sh", "application/x-sh");
            AddMapping(mappings, ".shar", "application/x-shar");
            AddMapping(mappings, ".sit", "application/x-stuffit");
            AddMapping(mappings, ".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12");
            AddMapping(mappings, ".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
            AddMapping(mappings, ".smd", "audio/x-smd");
            AddMapping(mappings, ".smi", "application/octet-stream");
            AddMapping(mappings, ".smx", "audio/x-smd");
            AddMapping(mappings, ".smz", "audio/x-smd");
            AddMapping(mappings, ".snd", "audio/basic");
            AddMapping(mappings, ".snp", "application/octet-stream");
            AddMapping(mappings, ".spc", "application/x-pkcs7-certificates");
            AddMapping(mappings, ".spl", "application/futuresplash");
            AddMapping(mappings, ".src", "application/x-wais-source");
            AddMapping(mappings, ".ssm", "application/streamingmedia");
            AddMapping(mappings, ".sst", "application/vnd.ms-pki.certstore");
            AddMapping(mappings, ".stl", "application/vnd.ms-pki.stl");
            AddMapping(mappings, ".sv4cpio", "application/x-sv4cpio");
            AddMapping(mappings, ".sv4crc", "application/x-sv4crc");
            AddMapping(mappings, ".swf", "application/x-shockwave-flash");
            AddMapping(mappings, ".t", "application/x-troff");
            AddMapping(mappings, ".tar", "application/x-tar");
            AddMapping(mappings, ".tcl", "application/x-tcl");
            AddMapping(mappings, ".tex", "application/x-tex");
            AddMapping(mappings, ".texi", "application/x-texinfo");
            AddMapping(mappings, ".texinfo", "application/x-texinfo");
            AddMapping(mappings, ".tgz", "application/x-compressed");
            AddMapping(mappings, ".thmx", "application/vnd.ms-officetheme");
            AddMapping(mappings, ".thn", "application/octet-stream");
            AddMapping(mappings, ".tif", "image/tiff");
            AddMapping(mappings, ".tiff", "image/tiff");
            AddMapping(mappings, ".toc", "application/octet-stream");
            AddMapping(mappings, ".tr", "application/x-troff");
            AddMapping(mappings, ".trm", "application/x-msterminal");
            AddMapping(mappings, ".tsv", "text/tab-separated-values");
            AddMapping(mappings, ".ttf", "application/octet-stream");
            AddMapping(mappings, ".txt", "text/plain");
            AddMapping(mappings, ".u32", "application/octet-stream");
            AddMapping(mappings, ".uls", "text/iuls");
            AddMapping(mappings, ".ustar", "application/x-ustar");
            AddMapping(mappings, ".vbs", "text/vbscript");
            AddMapping(mappings, ".vcf", "text/x-vcard");
            AddMapping(mappings, ".vcs", "text/plain");
            AddMapping(mappings, ".vdx", "application/vnd.ms-visio.viewer");
            AddMapping(mappings, ".vml", "text/xml");
            AddMapping(mappings, ".vsd", "application/vnd.visio");
            AddMapping(mappings, ".vss", "application/vnd.visio");
            AddMapping(mappings, ".vst", "application/vnd.visio");
            AddMapping(mappings, ".vsto", "application/x-ms-vsto");
            AddMapping(mappings, ".vsw", "application/vnd.visio");
            AddMapping(mappings, ".vsx", "application/vnd.visio");
            AddMapping(mappings, ".vtx", "application/vnd.visio");
            AddMapping(mappings, ".wav", "audio/wav");
            AddMapping(mappings, ".wax", "audio/x-ms-wax");
            AddMapping(mappings, ".wbmp", "image/vnd.wap.wbmp");
            AddMapping(mappings, ".wcm", "application/vnd.ms-works");
            AddMapping(mappings, ".wdb", "application/vnd.ms-works");
            AddMapping(mappings, ".wks", "application/vnd.ms-works");
            AddMapping(mappings, ".wm", "video/x-ms-wm");
            AddMapping(mappings, ".wma", "audio/x-ms-wma");
            AddMapping(mappings, ".wmd", "application/x-ms-wmd");
            AddMapping(mappings, ".wmf", "application/x-msmetafile");
            AddMapping(mappings, ".wml", "text/vnd.wap.wml");
            AddMapping(mappings, ".wmlc", "application/vnd.wap.wmlc");
            AddMapping(mappings, ".wmls", "text/vnd.wap.wmlscript");
            AddMapping(mappings, ".wmlsc", "application/vnd.wap.wmlscriptc");
            AddMapping(mappings, ".wmp", "video/x-ms-wmp");
            AddMapping(mappings, ".wmv", "video/x-ms-wmv");
            AddMapping(mappings, ".wmx", "video/x-ms-wmx");
            AddMapping(mappings, ".wmz", "application/x-ms-wmz");
            AddMapping(mappings, ".wps", "application/vnd.ms-works");
            AddMapping(mappings, ".wri", "application/x-mswrite");
            AddMapping(mappings, ".wrl", "x-world/x-vrml");
            AddMapping(mappings, ".wrz", "x-world/x-vrml");
            AddMapping(mappings, ".wsdl", "text/xml");
            AddMapping(mappings, ".wvx", "video/x-ms-wvx");
            AddMapping(mappings, ".x", "application/directx");
            AddMapping(mappings, ".xaf", "x-world/x-vrml");
            AddMapping(mappings, ".xaml", "application/xaml+xml");
            AddMapping(mappings, ".xap", "application/x-silverlight-app");
            AddMapping(mappings, ".xbap", "application/x-ms-xbap");
            AddMapping(mappings, ".xbm", "image/x-xbitmap");
            AddMapping(mappings, ".xdr", "text/plain");
            AddMapping(mappings, ".xla", "application/vnd.ms-excel");
            AddMapping(mappings, ".xlam", "application/vnd.ms-excel.addin.macroEnabled.12");
            AddMapping(mappings, ".xlc", "application/vnd.ms-excel");
            AddMapping(mappings, ".xlm", "application/vnd.ms-excel");
            AddMapping(mappings, ".xls", "application/vnd.ms-excel");
            AddMapping(mappings, ".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12");
            AddMapping(mappings, ".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12");
            AddMapping(mappings, ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            AddMapping(mappings, ".xlt", "application/vnd.ms-excel");
            AddMapping(mappings, ".xltm", "application/vnd.ms-excel.template.macroEnabled.12");
            AddMapping(mappings, ".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
            AddMapping(mappings, ".xlw", "application/vnd.ms-excel");
            AddMapping(mappings, ".xml", "text/xml");
            AddMapping(mappings, ".xof", "x-world/x-vrml");
            AddMapping(mappings, ".xpm", "image/x-xpixmap");
            AddMapping(mappings, ".xps", "application/vnd.ms-xpsdocument");
            AddMapping(mappings, ".xsd", "text/xml");
            AddMapping(mappings, ".xsf", "text/xml");
            AddMapping(mappings, ".xsl", "text/xml");
            AddMapping(mappings, ".xslt", "text/xml");
            AddMapping(mappings, ".xsn", "application/octet-stream");
            AddMapping(mappings, ".xtp", "application/octet-stream");
            AddMapping(mappings, ".xwd", "image/x-xwindowdump");
            AddMapping(mappings, ".z", "application/x-compress");
            AddMapping(mappings, ".zip", "application/x-zip-compressed");
            AddMapping(mappings, ".apk", "application/vnd.android");
            mappings.Clear();
            mappings = null;
        }
        private ResourceHandler(string type)
        {
            _type = type;
        }

        private static void AddMapping(Dictionary<string, ResourceHandler> mappings, string key, string value)
        {
            ResourceHandler handler;
            if (!mappings.TryGetValue(value, out handler))
            {
                handler = new ResourceHandler(value);
                mappings.Add(value, handler);
            }
            if (!object.ReferenceEquals(Default, handler))
                _mappings.Add(key, handler);
        }

        public static ResourceHandler Parse(ExtensionType extType, string ext, bool html)
        {
            if (extType == ExtensionType.Html)
                return html ? Html : null;

            ResourceHandler handler;
            if (_mappings.TryGetValue(ext, out handler))
                return handler;
            return Default;
        }

        public string ContentType
        {
            get { return _type; }
        }

        public void ProcessRequest(HttpContext context, string path)
        {
            //string origin = context.Request.Headers["Origin"];
            //if (!string.IsNullOrEmpty(origin))
            //    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(DateTime.MaxValue - DateTime.Now);
            DateTime v = (new FileInfo(path)).LastWriteTime;
            string ticks = v.Ticks.ToString();
            context.Response.Cache.SetETag(ticks);
            bool hascache = false;
            string etag = context.Request.Headers["If-None-Match"];
            if (!string.IsNullOrEmpty(etag))
                hascache = string.Equals(ticks, etag);
            if (hascache)
            {
                context.Response.StatusCode = 304;
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = ContentType;
                context.Response.Cache.SetLastModified(v);
                context.Response.WriteFile(path);
            }
            context.Response.End();
        }
        //public void BeginProcessRequest(HttpContext context, string path, AsyncCallback cb, object data)
        //{

        //}
        //public void EndProcessRequest(IAsyncResult result)
        //{

        //}
        public void ProcessRequest(HttpContext context, string[] paths)
        {
            //string origin = context.Request.Headers["Origin"];
            //if (!string.IsNullOrEmpty(origin))
            //    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(DateTime.MaxValue - DateTime.Now);
            DateTime t;
            DateTime v = DateTime.MinValue;
            StringBuilder sb = new StringBuilder(8 * paths.Length);
            for (int i = 0; i < paths.Length; ++i)
            {
                try
                {
                    t = (new FileInfo(paths[i])).LastWriteTime;
                    if (i > 0)
                        sb.Append('.');
                    if (t > v)
                        v = t;
                    sb.Append(t.Ticks.ToString());
                }
                catch (Exception) { }
            }
            string ticks = sb.ToString();
            context.Response.Cache.SetETag(ticks);
            bool hascache = false;
            string etag = context.Request.Headers["If-None-Match"];
            if (!string.IsNullOrEmpty(etag))
                hascache = string.Equals(ticks, etag);
            if (hascache)
            {
                context.Response.StatusCode = 304;
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = ContentType;
                context.Response.Cache.SetLastModified(v);
                foreach (string path in paths)
                {
                    try
                    {
                        context.Response.WriteFile(path);
                    }
                    catch (Exception) { }
                }
            }
            context.Response.End();
        }
        //public void BeginProcessRequest(HttpContext context, string[] paths, AsyncCallback cb, object data)
        //{

        //}
    }
}
