using Cnaws.Product.Logistics.Providers;
using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Cnaws.Product.Logistics
{
    public interface ILogisticsInfo
    {
        DateTime Time { get; }
        string Status { get; }
    }

    public enum HttpMethod
    {
        Get,
        Post
    }

    [Serializable]
    public sealed class LogisticsProviderItem
    {
        public string Key = null;
        public string Name = null;
    }

    [Serializable]
    public sealed class LogisticsInfoItem
    {
        public string Time = null;
        public string Status = null;
    }

    public abstract class LogisticsProvider
    {
        private static readonly Regex ElementBeginRegex = new Regex(@"<\w+(\s+[^>]*)?>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex ElementEndRegex = new Regex(@"</\w+>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static readonly List<LogisticsProviderItem> ProviderList;

        static LogisticsProvider()
        {
            LogisticsProvider provider;
            ProviderList = new List<LogisticsProviderItem>();
            string ns = string.Concat("Cnaws.Product.Logistics.Providers");
            Assembly asm = Assembly.GetAssembly(TType<LogisticsProvider>.Type);
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                if (type.IsClass &&
                    !type.IsAbstract &&
                    string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase) &&
                    TType<LogisticsProvider>.Type.IsAssignableFrom(type.UnderlyingSystemType))
                {
                    provider = (LogisticsProvider)Activator.CreateInstance(type.UnderlyingSystemType);
                    ProviderList.Add(new LogisticsProviderItem() { Key = provider.Key, Name = provider.Name });
                }
            }
        }

        public static List<LogisticsProviderItem> Providers
        {
            get { return ProviderList; }
        }

        public static LogisticsProvider Create(string name)
        {
            try
            {
                Type type = Type.GetType(string.Concat("Cnaws.Product.Logistics.Providers.", name, ",Cnaws.Product"), true, true);
                object result = Activator.CreateInstance(type);
                if (TType<LogisticsProvider>.Type.IsAssignableFrom(result.GetType()))
                    return (LogisticsProvider)result;
            }
            catch (Exception) { }
            return null;
        }

        public string Key
        {
            get { return GetType().Name.ToLower(); }
        }
        public abstract string Name { get; }
        public virtual Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }
        public abstract string SearchUrl { get; }
        public virtual string Charset
        {
            get { return "utf-8"; }
        }
        public abstract ILogisticsInfo[] ParseResult(string s);


        public virtual HttpMethod HttpMethod
        {
            get { return HttpMethod.Get; }
        }
        public virtual string PostArguments
        {
            get { return null; }
        }
        public virtual string RefererUrl
        {
            get { return null; }
        }
        public virtual string UserAgent
        {
            get { return "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0"; }
        }

        public LogisticsInfoItem[] Search(string order)
        {
            string result;
            byte[] data = null;
            Encoding charset = Encoding.GetEncoding(Charset);
            string url = string.Format(SearchUrl, order);
            if (HttpMethod == HttpMethod.Post)
                data = charset.GetBytes(string.Format(PostArguments, order));
            if (!HttpRequest(url, out result, data, charset))
                throw new Exception();
            LogisticsInfoItem[] array= Array.ConvertAll(ParseResult(result), new Converter<ILogisticsInfo, LogisticsInfoItem>((x) => new LogisticsInfoItem() { Time = x.Time.ToString(), Status = x.Status }));
            foreach (LogisticsInfoItem item in array)
                item.Status = ElementEndRegex.Replace(ElementBeginRegex.Replace(item.Status, string.Empty), string.Empty);
            return array;
        }

        private bool HttpRequest(string url, out string result, byte[] data, Encoding charset = null, int timeout = 120000)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Timeout = timeout;
                if (!string.IsNullOrEmpty(UserAgent))
                    request.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(RefererUrl))
                    request.Referer = RefererUrl;
                if (data != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;
                    using (Stream s = request.GetRequestStream())
                        s.Write(data, 0, data.Length);
                }
                else
                {
                    request.Method = "GET";
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream s = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(s, charset ?? Encoding.UTF8))
                            result = reader.ReadToEnd();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                result = string.Concat(ex.Message, Environment.NewLine, ex.StackTrace);
            }
            return false;
        }
    }
}
