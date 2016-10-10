using System;
using System.Configuration;
using Cnaws.Templates;
using Cnaws.Configuration;
using System.Web.Configuration;

namespace Cnaws.Web.Configuration
{
    public sealed class SMSCaptchaSection : ConfigurationSection
    {
        private const string CharsName = "chars";
        private const string DefaultCountName = "defaultCount";
        private const string TimeSpanName = "timeSpan";
        private const string ExpirationName = "expiration";

        private static readonly ConfigurationProperty _propChars = new ConfigurationProperty(CharsName, TType<string>.Type, Utility.SMSCaptchaChars, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultCount = new ConfigurationProperty(DefaultCountName, TType<int>.Type, Utility.SMSCaptchaDefaultCount, null, new IntegerValidator(4, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propTimeSpan = new ConfigurationProperty(TimeSpanName, TType<int>.Type, Utility.SMSCaptchaTimeSpan, null, new IntegerValidator(30, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propExpiration = new ConfigurationProperty(ExpirationName, TType<int>.Type, Utility.SMSCaptchaExpiration, null, new IntegerValidator(60, int.MaxValue), ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static SMSCaptchaSection()
        {
            _properties.Add(_propChars);
            _properties.Add(_propDefaultCount);
            _properties.Add(_propTimeSpan);
            _properties.Add(_propExpiration);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty(CharsName, DefaultValue = Utility.SMSCaptchaChars)]
        public string Chars
        {
            get { return (string)base[_propChars]; }
            set { this[_propChars] = value; }
        }
        [ConfigurationProperty(DefaultCountName, DefaultValue = Utility.SMSCaptchaDefaultCount)]
        public int DefaultCount
        {
            get { return (int)base[_propDefaultCount]; }
            set { this[_propDefaultCount] = value; }
        }
        [ConfigurationProperty(TimeSpanName, DefaultValue = Utility.SMSCaptchaTimeSpan)]
        public int TimeSpan
        {
            get { return (int)base[_propTimeSpan]; }
            set { this[_propTimeSpan] = value; }
        }
        [ConfigurationProperty(ExpirationName, DefaultValue = Utility.SMSCaptchaExpiration)]
        public int Expiration
        {
            get { return (int)base[_propExpiration]; }
            set { this[_propExpiration] = value; }
        }

        public static SMSCaptchaSection GetSection()
        {
            return (SMSCaptchaSection)WebConfigurationManager.GetSection("system.web/smscaptcha");
        }
        public static SMSCaptchaSection GetSection(System.Configuration.Configuration config)
        {
            return (SMSCaptchaSection)config.GetSection("system.web/smscaptcha");
        }
    }
}
