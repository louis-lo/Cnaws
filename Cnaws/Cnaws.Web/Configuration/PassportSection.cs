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
    public enum PassportLevel
    {
        Low = 0,
        Normal,
        High
    }

    internal sealed class PassportLevelConverter : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return Enum.Parse(TType<PassportLevel>.Type, (string)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Version version = (Version)value;
            return version.ToString();
        }
    }

    public sealed class PassportSection : ConfigurationSection
    {
        private const string CookieNameName = "cookieName";
        private const string CookieDomainName = "cookieDomain";
        private const string CookieIVName = "cookieIV";
        private const string CookieKeyName = "cookieKey";
        private const string MaxInvalidPasswordAttemptsName = "maxInvalidPasswordAttempts";
        private const string PasswordAnswerAttemptLockoutDurationName = "passwordAnswerAttemptLockoutDuration";
        private const string LevelName = "level";
        private const string VerifyMailName = "verifyMail";
        private const string VerifyMobileName = "verifyMobile";
        private const string LoginWithCaptchaName = "loginWithCaptcha";
        private const string RegisterWithCaptchaName = "registerWithCaptcha";
        private const string DefaultApprovedName = "defaultApproved";
        private const string DataProviderName = "dataApproved";

        private static readonly ConfigurationProperty _propCookieName = new ConfigurationProperty(CookieNameName, TType<string>.Type, Utility.PassportCookieName, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieDomain = new ConfigurationProperty(CookieDomainName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieIV = new ConfigurationProperty(CookieIVName, TType<string>.Type, Utility.PassportCookieIV, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieKey = new ConfigurationProperty(CookieKeyName, TType<string>.Type, Utility.PassportCookieKey, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxInvalidPasswordAttempts = new ConfigurationProperty(MaxInvalidPasswordAttemptsName, TType<int>.Type, Utility.PassportMaxInvalidPasswordAttempts, null, new IntegerValidator(0, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPasswordAnswerAttemptLockoutDuration = new ConfigurationProperty(PasswordAnswerAttemptLockoutDurationName, TType<int>.Type, Utility.PassportPasswordAnswerAttemptLockoutDuration, null, new IntegerValidator(0, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propLevel = new ConfigurationProperty(LevelName, TType<PassportLevel>.Type, PassportLevel.Low, EnumConverter<PassportLevel>.Instance, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propVerifyMail = new ConfigurationProperty(VerifyMailName, TType<bool>.Type, false, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propVerifyMobile = new ConfigurationProperty(VerifyMobileName, TType<bool>.Type, false, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propLoginWithCaptcha = new ConfigurationProperty(LoginWithCaptchaName, TType<bool>.Type, false, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRegisterWithCaptcha = new ConfigurationProperty(RegisterWithCaptchaName, TType<bool>.Type, false, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultApproved = new ConfigurationProperty(DefaultApprovedName, TType<bool>.Type, true, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDataProvider = new ConfigurationProperty(DataProviderName, TType<string>.Type, DataUtility.DefaultProvider, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static PassportSection()
        {
            _properties.Add(_propCookieName);
            _properties.Add(_propCookieDomain);
            _properties.Add(_propCookieIV);
            _properties.Add(_propCookieKey);
            _properties.Add(_propMaxInvalidPasswordAttempts);
            _properties.Add(_propPasswordAnswerAttemptLockoutDuration);
            _properties.Add(_propLevel);
            _properties.Add(_propVerifyMail);
            _properties.Add(_propVerifyMobile);
            _properties.Add(_propLoginWithCaptcha);
            _properties.Add(_propRegisterWithCaptcha);
            _properties.Add(_propDefaultApproved);
            _properties.Add(_propDataProvider);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty(CookieNameName, DefaultValue = Utility.PassportCookieName)]
        public string CookieName
        {
            get { return (string)base[_propCookieName]; }
            set { this[_propCookieName] = value; }
        }
        [ConfigurationProperty(CookieDomainName, DefaultValue = "")]
        public string CookieDomain
        {
            get { return (string)base[_propCookieDomain]; }
            set { this[_propCookieDomain] = value; }
        }
        [ConfigurationProperty(CookieIVName, DefaultValue = Utility.PassportCookieIV)]
        public string CookieIV
        {
            get { return (string)base[_propCookieIV]; }
            set { this[_propCookieIV] = value; }
        }
        [ConfigurationProperty(CookieKeyName, DefaultValue = Utility.PassportCookieKey)]
        public string CookieKey
        {
            get { return (string)base[_propCookieKey]; }
            set { this[_propCookieKey] = value; }
        }
        [ConfigurationProperty(MaxInvalidPasswordAttemptsName, DefaultValue = Utility.PassportMaxInvalidPasswordAttempts)]
        public int MaxInvalidPasswordAttempts
        {
            get { return (int)base[_propMaxInvalidPasswordAttempts]; }
            set { this[_propMaxInvalidPasswordAttempts] = value; }
        }
        [ConfigurationProperty(PasswordAnswerAttemptLockoutDurationName, DefaultValue = Utility.PassportPasswordAnswerAttemptLockoutDuration)]
        public int PasswordAnswerAttemptLockoutDuration
        {
            get { return (int)base[_propPasswordAnswerAttemptLockoutDuration]; }
            set { this[_propPasswordAnswerAttemptLockoutDuration] = value; }
        }
        [ConfigurationProperty(LevelName, DefaultValue = PassportLevel.Low)]
        public PassportLevel Level
        {
            get { return (PassportLevel)base[_propLevel]; }
            set { this[_propLevel] = value; }
        }
        [ConfigurationProperty(VerifyMailName, DefaultValue = false)]
        public bool VerifyMail
        {
            get { return (bool)base[_propVerifyMail]; }
            set { this[_propVerifyMail] = value; }
        }
        [ConfigurationProperty(VerifyMobileName, DefaultValue = false)]
        public bool VerifyMobile
        {
            get { return (bool)base[_propVerifyMobile]; }
            set { this[_propVerifyMobile] = value; }
        }
        [ConfigurationProperty(LoginWithCaptchaName, DefaultValue = false)]
        public bool LoginWithCaptcha
        {
            get { return (bool)base[_propLoginWithCaptcha]; }
            set { this[_propLoginWithCaptcha] = value; }
        }
        [ConfigurationProperty(RegisterWithCaptchaName, DefaultValue = false)]
        public bool RegisterWithCaptcha
        {
            get { return (bool)base[_propRegisterWithCaptcha]; }
            set { this[_propRegisterWithCaptcha] = value; }
        }
        [ConfigurationProperty(DefaultApprovedName, DefaultValue = true)]
        public bool DefaultApproved
        {
            get { return (bool)base[_propDefaultApproved]; }
            set { this[_propDefaultApproved] = value; }
        }
        [ConfigurationProperty(DataProviderName, DefaultValue = DataUtility.DefaultProvider)]
        public string DataProvider
        {
            get { return (string)base[_propDataProvider]; }
            set { this[_propDataProvider] = value; }
        }

        public static PassportSection GetSection()
        {
            return (PassportSection)WebConfigurationManager.GetSection("system.web/passport");
        }
        public static PassportSection GetSection(System.Configuration.Configuration config)
        {
            return (PassportSection)config.GetSection("system.web/passport");
        }
    }
}
