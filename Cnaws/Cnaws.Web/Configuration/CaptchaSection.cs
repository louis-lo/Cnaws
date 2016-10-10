using System;
using System.Configuration;
using Cnaws.Templates;
using Cnaws.Configuration;
using System.Web.Configuration;

namespace Cnaws.Web.Configuration
{
    public sealed class CaptchaSection : ConfigurationSection
    {
        private const string CharsName = "chars";
        private const string CookiePrefixName = "cookiePrefix";
        private const string CookieDomainName = "cookieDomain";
        private const string DefaultWidthName = "defaultWidth";
        private const string DefaultHeightName = "defaultHeight";
        private const string DefaultCountName = "defaultCount";
        private const string ExpirationName = "expiration";

        private static readonly ConfigurationProperty _propChars = new ConfigurationProperty(CharsName, TType<string>.Type, Utility.CaptchaChars, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookiePrefix = new ConfigurationProperty(CookiePrefixName, TType<string>.Type, Utility.CaptchaCookieName, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieDomain = new ConfigurationProperty(CookieDomainName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultWidth = new ConfigurationProperty(DefaultWidthName, TType<int>.Type, Utility.CaptchaDefaultWidth, null, new IntegerValidator(20, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultHeight = new ConfigurationProperty(DefaultHeightName, TType<int>.Type, Utility.CaptchaDefaultHeight, null, new IntegerValidator(20, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultCount = new ConfigurationProperty(DefaultCountName, TType<int>.Type, Utility.CaptchaDefaultCount, null, new IntegerValidator(4, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propExpiration = new ConfigurationProperty(ExpirationName, TType<int>.Type, Utility.CaptchaExpiration, null, new IntegerValidator(1, int.MaxValue), ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static CaptchaSection()
        {
            _properties.Add(_propChars);
            _properties.Add(_propCookiePrefix);
            _properties.Add(_propCookieDomain);
            _properties.Add(_propDefaultWidth);
            _properties.Add(_propDefaultHeight);
            _properties.Add(_propDefaultCount);
            _properties.Add(_propExpiration);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty(CharsName, DefaultValue = Utility.CaptchaChars)]
        public string Chars
        {
            get { return (string)base[_propChars]; }
            set { this[_propChars] = value; }
        }
        [ConfigurationProperty(CookiePrefixName, DefaultValue = Utility.CaptchaCookieName)]
        public string CookiePrefix
        {
            get { return (string)base[_propCookiePrefix]; }
            set { this[_propCookiePrefix] = value; }
        }
        [ConfigurationProperty(CookieDomainName, DefaultValue = "")]
        public string CookieDomain
        {
            get { return (string)base[_propCookieDomain]; }
            set { this[_propCookieDomain] = value; }
        }
        [ConfigurationProperty(DefaultWidthName, DefaultValue = Utility.CaptchaDefaultWidth)]
        public int DefaultWidth
        {
            get { return (int)base[_propDefaultWidth]; }
            set { this[_propDefaultWidth] = value; }
        }
        [ConfigurationProperty(DefaultHeightName, DefaultValue = Utility.CaptchaDefaultHeight)]
        public int DefaultHeight
        {
            get { return (int)base[_propDefaultHeight]; }
            set { this[_propDefaultHeight] = value; }
        }
        [ConfigurationProperty(DefaultCountName, DefaultValue = Utility.CaptchaDefaultCount)]
        public int DefaultCount
        {
            get { return (int)base[_propDefaultCount]; }
            set { this[_propDefaultCount] = value; }
        }
        [ConfigurationProperty(ExpirationName, DefaultValue = Utility.CaptchaExpiration)]
        public int Expiration
        {
            get { return (int)base[_propExpiration]; }
            set { this[_propExpiration] = value; }
        }

        public static CaptchaSection GetSection()
        {
            return (CaptchaSection)WebConfigurationManager.GetSection("system.web/captcha");
        }
        public static CaptchaSection GetSection(System.Configuration.Configuration config)
        {
            return (CaptchaSection)config.GetSection("system.web/captcha");
        }
    }
}
