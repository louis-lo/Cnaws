using System;
using System.Configuration;
using System.Globalization;
using System.ComponentModel;
using Cnaws.Templates;
using Cnaws.Configuration;
using System.Web.Configuration;
using Cnaws.Data;

namespace Cnaws.Web.Configuration
{
    public enum SiteUrlMode
    {
        Rewrite = 0,
        Static
    }
    public enum CacheMode
    {
        Application = 0,
        File = 1,
        MMFile = 2,
        Sql = 3,
    }

    public sealed class SiteSection : ConfigurationSection
    {
        private const string ThemeName = "theme";
        private const string UrlModeName = "urlMode";
        //private const string UrlExtName = "urlExt";
        private const string ManagementName = "management";
        private const string WapDomainName = "wapDomain";
        private const string SubDomainName = "subDomain";
        private const string DataProviderName = "dataProvider";
        private const string CacheModeName = "cacheMode";
        private const string CacheProviderName = "cacheProvider";
        private const string ResourcesName = "resources";
        private const string PassportName = "passport";
        private const string WapPassportName = "wapPassport";

        private static readonly ConfigurationProperty _propTheme = new ConfigurationProperty(ThemeName, TType<string>.Type, Utility.DefaultTheme, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUrlMode = new ConfigurationProperty(UrlModeName, TType<SiteUrlMode>.Type, SiteUrlMode.Rewrite, EnumConverter<SiteUrlMode>.Instance, null, ConfigurationPropertyOptions.None);
        //private static readonly ConfigurationProperty _propUrlExt = new ConfigurationProperty(UrlExtName, TType<string>.Type, Utility.DefaultExt, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propManagement = new ConfigurationProperty(ManagementName, TType<string>.Type, Utility.DefaultManagement, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propWapDomain = new ConfigurationProperty(WapDomainName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSubDomain = new ConfigurationProperty(SubDomainName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDataProvider = new ConfigurationProperty(DataProviderName, TType<string>.Type, DataUtility.DefaultProvider, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCacheMode = new ConfigurationProperty(CacheModeName, TType<CacheMode>.Type, CacheMode.Application, EnumConverter<CacheMode>.Instance, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCacheProvider = new ConfigurationProperty(CacheProviderName, TType<string>.Type, DataUtility.DefaultProvider, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propResources = new ConfigurationProperty(ResourcesName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPassport = new ConfigurationProperty(PassportName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propWapPassport = new ConfigurationProperty(WapPassportName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static SiteSection()
        {
            _properties.Add(_propTheme);
            _properties.Add(_propUrlMode);
            //_properties.Add(_propUrlExt);
            _properties.Add(_propManagement);
            _properties.Add(_propWapDomain);
            _properties.Add(_propSubDomain);
            _properties.Add(_propDataProvider);
            _properties.Add(_propCacheMode);
            _properties.Add(_propCacheProvider);
            _properties.Add(_propResources);
            _properties.Add(_propPassport);
            _properties.Add(_propWapPassport);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty(ThemeName, DefaultValue = Utility.DefaultTheme)]
        public string Theme
        {
            get { return (string)base[_propTheme]; }
            set { this[_propTheme] = value; }
        }

        [ConfigurationProperty(UrlModeName, DefaultValue = SiteUrlMode.Rewrite)]
        public SiteUrlMode UrlMode
        {
            get { return (SiteUrlMode)base[_propUrlMode]; }
            set { this[_propUrlMode] = value; }
        }

        //[ConfigurationProperty(UrlExtName, DefaultValue = Utility.DefaultExt)]
        //public string UrlExt
        //{
        //    get { return (string)base[_propUrlExt]; }
        //    set { this[_propUrlExt] = value; }
        //}

        [ConfigurationProperty(ManagementName, DefaultValue = Utility.DefaultManagement)]
        public string Management
        {
            get { return (string)base[_propManagement]; }
            set { this[_propManagement] = value; }
        }

        [ConfigurationProperty(WapDomainName, DefaultValue = "")]
        public string WapDomain
        {
            get { return (string)base[_propWapDomain]; }
            set { this[_propWapDomain] = value; }
        }
        [ConfigurationProperty(SubDomainName, DefaultValue = "")]
        public string SubDomain
        {
            get { return (string)base[_propSubDomain]; }
            set { this[_propSubDomain] = value; }
        }
        [ConfigurationProperty(DataProviderName, DefaultValue = DataUtility.DefaultProvider)]
        public string DataProvider
        {
            get { return (string)base[_propDataProvider]; }
            set { this[_propDataProvider] = value; }
        }
        [ConfigurationProperty(CacheModeName, DefaultValue = CacheMode.Application)]
        public CacheMode CacheMode
        {
            get { return (CacheMode)base[_propCacheMode]; }
            set { this[_propCacheMode] = value; }
        }
        [ConfigurationProperty(CacheProviderName, DefaultValue = DataUtility.DefaultProvider)]
        public string CacheProvider
        {
            get { return (string)base[_propCacheProvider]; }
            set { this[_propCacheProvider] = value; }
        }
        [ConfigurationProperty(ResourcesName, DefaultValue = "")]
        public string ResourcesUrl
        {
            get { return (string)base[_propResources]; }
            set { this[_propResources] = value; }
        }
        [ConfigurationProperty(PassportName, DefaultValue = "")]
        public string PassportUrl
        {
            get { return (string)base[_propPassport]; }
            set { this[_propPassport] = value; }
        }
        [ConfigurationProperty(WapPassportName, DefaultValue = "")]
        public string WapPassportUrl
        {
            get { return (string)base[_propWapPassport]; }
            set { this[_propWapPassport] = value; }
        }

        public static SiteSection GetSection()
        {
            return (SiteSection)WebConfigurationManager.GetSection("system.web/site");
        }
        public static SiteSection GetSection(System.Configuration.Configuration config)
        {
            return (SiteSection)config.GetSection("system.web/site");
        }
    }
}
