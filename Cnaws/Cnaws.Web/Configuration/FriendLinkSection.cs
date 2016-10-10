using System;
using Cnaws.Templates;
using System.Configuration;
using Cnaws.Configuration;
using System.Web.Configuration;

namespace Cnaws.Web.Configuration
{
    public enum FriendLinkMode
    {
        Text,
        Image
    }

    public sealed class FriendLinkSection : ConfigurationSection
    {
        private const string EnableName = "enable";
        private const string ApprovedName = "approved";
        private const string ModeName = "mode";
        
        private static readonly ConfigurationProperty _propEnable = new ConfigurationProperty(EnableName, TType<bool>.Type, true, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propApproved = new ConfigurationProperty(ApprovedName, TType<bool>.Type, true, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMode = new ConfigurationProperty(ModeName, TType<FriendLinkMode>.Type, FriendLinkMode.Text, EnumConverter<FriendLinkMode>.Instance, null, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static FriendLinkSection()
        {
            _properties.Add(_propEnable);
            _properties.Add(_propApproved);
            _properties.Add(_propMode);
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

        [ConfigurationProperty(ApprovedName, DefaultValue = true)]
        public bool Approved
        {
            get { return (bool)base[_propApproved]; }
            set { this[_propApproved] = value; }
        }

        [ConfigurationProperty(ModeName, DefaultValue = FriendLinkMode.Text)]
        public FriendLinkMode Mode
        {
            get { return (FriendLinkMode)base[_propMode]; }
            set { this[_propMode] = value; }
        }

        public static FriendLinkSection GetSection()
        {
            return (FriendLinkSection)WebConfigurationManager.GetSection("system.web/friendlink");
        }
        public static FriendLinkSection GetSection(System.Configuration.Configuration config)
        {
            return (FriendLinkSection)config.GetSection("system.web/friendlink");
        }
    }
}
