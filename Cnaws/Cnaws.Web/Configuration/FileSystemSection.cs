using System;
using System.Configuration;
using Cnaws.Templates;
using Cnaws.Configuration;
using System.Web.Configuration;

namespace Cnaws.Web
{
    public enum ImageMarkType
    {
        None = 0,
        Text,
        Image
    }
    public enum ImageMarkRegion
    {
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
namespace Cnaws.Web.Configuration
{
    public sealed class FileSystemSection : ConfigurationSection
    {
        private const string EnableName = "enable";
        private const string PathName = "path";
        private const string UrlName = "url";
        private const string MarkName = "mark";
        private const string TextName = "text";
        private const string RegionName = "region";
        private const string WidthName = "width";
        private const string HeightName = "height";

        private static readonly ConfigurationProperty _propEnable = new ConfigurationProperty(EnableName, TType<bool>.Type, true, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPath = new ConfigurationProperty(PathName, TType<string>.Type, Utility.UploadDir, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUrl = new ConfigurationProperty(UrlName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMark = new ConfigurationProperty(MarkName, TType<ImageMarkType>.Type, ImageMarkType.None, EnumConverter<ImageMarkType>.Instance, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propText = new ConfigurationProperty(TextName, TType<string>.Type, string.Empty, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRegion = new ConfigurationProperty(RegionName, TType<ImageMarkRegion>.Type, ImageMarkRegion.BottomRight, EnumConverter<ImageMarkRegion>.Instance, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propWidth = new ConfigurationProperty(WidthName, TType<int>.Type, 0, null, new IntegerValidator(0, int.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propHeight = new ConfigurationProperty(HeightName, TType<int>.Type, 0, null, new IntegerValidator(0, int.MaxValue), ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static FileSystemSection()
        {
            _properties.Add(_propEnable);
            _properties.Add(_propPath);
            _properties.Add(_propUrl);
            _properties.Add(_propMark);
            _properties.Add(_propText);
            _properties.Add(_propRegion);
            _properties.Add(_propWidth);
            _properties.Add(_propHeight);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty(EnableName, DefaultValue = true)]
        public bool Enable
        {
            get { return (bool)base[_propEnable]; }
            set { this[_propEnable] = value; }
        }
        [ConfigurationProperty(PathName, DefaultValue = Utility.UploadDir)]
        public string Path
        {
            get { return (string)base[_propPath]; }
            set { this[_propPath] = value; }
        }
        [ConfigurationProperty(UrlName, DefaultValue = "")]
        public string Url
        {
            get { return (string)base[_propUrl]; }
            set { this[_propUrl] = value; }
        }
        [ConfigurationProperty(MarkName, DefaultValue = ImageMarkType.None)]
        public ImageMarkType Mark
        {
            get { return (ImageMarkType)base[_propMark]; }
            set { this[_propMark] = value; }
        }
        [ConfigurationProperty(TextName, DefaultValue = "")]
        public string Text
        {
            get { return (string)base[_propText]; }
            set { this[_propText] = value; }
        }
        [ConfigurationProperty(RegionName, DefaultValue = ImageMarkRegion.BottomRight)]
        public ImageMarkRegion Region
        {
            get { return (ImageMarkRegion)base[_propRegion]; }
            set { this[_propRegion] = value; }
        }
        [ConfigurationProperty(WidthName, DefaultValue = 0)]
        public int Width
        {
            get { return (int)base[_propWidth]; }
            set { this[_propWidth] = value; }
        }
        [ConfigurationProperty(HeightName, DefaultValue = 0)]
        public int Height
        {
            get { return (int)base[_propHeight]; }
            set { this[_propHeight] = value; }
        }

        public static FileSystemSection GetSection()
        {
            return (FileSystemSection)WebConfigurationManager.GetSection("system.web/filesystem");
        }
        public static FileSystemSection GetSection(System.Configuration.Configuration config)
        {
            return (FileSystemSection)config.GetSection("system.web/filesystem");
        }
    }
}
