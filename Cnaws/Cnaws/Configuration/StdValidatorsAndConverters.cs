using System;
using System.Configuration;
using System.Globalization;
using System.ComponentModel;
using Cnaws.Templates;

namespace Cnaws.Configuration
{
    public static class StdValidatorsAndConverters
    {
        private static TypeConverter s_infiniteTimeSpanConverter;
        private static ConfigurationValidatorBase s_nonEmptyStringValidator;
        private static ConfigurationValidatorBase s_nonZeroPositiveIntegerValidator;
        private static ConfigurationValidatorBase s_positiveIntegerValidator;
        private static ConfigurationValidatorBase s_nonZeroIntegerValidator;
        private static ConfigurationValidatorBase s_positiveTimeSpanValidator;
        private static TypeConverter s_timeSpanMinutesConverter;
        private static TypeConverter s_timeSpanMinutesOrInfiniteConverter;
        private static TypeConverter s_timeSpanSecondsConverter;
        private static TypeConverter s_timeSpanSecondsOrInfiniteConverter;
        private static TypeConverter s_versionConverter;
        private static TypeConverter s_whiteSpaceTrimStringConverter;

        public static TypeConverter InfiniteTimeSpanConverter
        {
            get
            {
                if (s_infiniteTimeSpanConverter == null)
                {
                    s_infiniteTimeSpanConverter = new InfiniteTimeSpanConverter();
                }
                return s_infiniteTimeSpanConverter;
            }
        }

        public static ConfigurationValidatorBase NonEmptyStringValidator
        {
            get
            {
                if (s_nonEmptyStringValidator == null)
                {
                    s_nonEmptyStringValidator = new StringValidator(1);
                }
                return s_nonEmptyStringValidator;
            }
        }

        public static ConfigurationValidatorBase NonZeroPositiveIntegerValidator
        {
            get
            {
                if (s_nonZeroPositiveIntegerValidator == null)
                {
                    s_nonZeroPositiveIntegerValidator = new IntegerValidator(1, 0x7fffffff);
                }
                return s_nonZeroPositiveIntegerValidator;
            }
        }

        public static ConfigurationValidatorBase PositiveIntegerValidator
        {
            get
            {
                if (s_positiveIntegerValidator == null)
                {
                    s_positiveIntegerValidator = new IntegerValidator(0, 0x7fffffff);
                }
                return s_positiveIntegerValidator;
            }
        }

        public static ConfigurationValidatorBase NonZeroIntegerValidator
        {
            get
            {
                if (s_nonZeroIntegerValidator == null)
                {
                    s_nonZeroIntegerValidator = new IntegerValidator(1, 0x7fffffff);
                }
                return s_nonZeroIntegerValidator;
            }
        }

        public static ConfigurationValidatorBase PositiveTimeSpanValidator
        {
            get
            {
                if (s_positiveTimeSpanValidator == null)
                {
                    s_positiveTimeSpanValidator = new PositiveTimeSpanValidator();
                }
                return s_positiveTimeSpanValidator;
            }
        }

        public static TypeConverter TimeSpanMinutesConverter
        {
            get
            {
                if (s_timeSpanMinutesConverter == null)
                {
                    s_timeSpanMinutesConverter = new TimeSpanMinutesConverter();
                }
                return s_timeSpanMinutesConverter;
            }
        }

        public static TypeConverter TimeSpanMinutesOrInfiniteConverter
        {
            get
            {
                if (s_timeSpanMinutesOrInfiniteConverter == null)
                {
                    s_timeSpanMinutesOrInfiniteConverter = new TimeSpanMinutesOrInfiniteConverter();
                }
                return s_timeSpanMinutesOrInfiniteConverter;
            }
        }

        public static TypeConverter TimeSpanSecondsConverter
        {
            get
            {
                if (s_timeSpanSecondsConverter == null)
                {
                    s_timeSpanSecondsConverter = new TimeSpanSecondsConverter();
                }
                return s_timeSpanSecondsConverter;
            }
        }

        public static TypeConverter TimeSpanSecondsOrInfiniteConverter
        {
            get
            {
                if (s_timeSpanSecondsOrInfiniteConverter == null)
                {
                    s_timeSpanSecondsOrInfiniteConverter = new TimeSpanSecondsOrInfiniteConverter();
                }
                return s_timeSpanSecondsOrInfiniteConverter;
            }
        }

        public static TypeConverter VersionConverter
        {
            get
            {
                if (s_versionConverter == null)
                {
                    s_versionConverter = new VersionConverter();
                }
                return s_versionConverter;
            }
        }

        public static TypeConverter WhiteSpaceTrimStringConverter
        {
            get
            {
                if (s_whiteSpaceTrimStringConverter == null)
                {
                    s_whiteSpaceTrimStringConverter = new WhiteSpaceTrimStringConverter();
                }
                return s_whiteSpaceTrimStringConverter;
            }
        }
    }

    public sealed class VersionConverter : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new Version((string)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Version version = (Version)value;
            return version.ToString();
        }
    }

    public sealed class EnumConverter<T> : ConfigurationConverterBase
    {
        private static EnumConverter<T> s_instance;

        static EnumConverter()
        {
            s_instance = null;
        }

        public static EnumConverter<T> Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new EnumConverter<T>();
                }
                return s_instance;
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = (string)value;
            if (!string.IsNullOrEmpty(s))
                return Enum.Parse(TType<T>.Type, (string)value);
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}
