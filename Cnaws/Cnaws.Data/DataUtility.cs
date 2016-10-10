using System;
using Cnaws.Templates;

namespace Cnaws.Data
{
    public static class DataUtility
    {
        public const string DefaultProvider = "LocalSqlServer";

        public static object FromDataType(object value, Type conversionType)
        {
            if (value != null)
            {
                if (!(value is DBNull))
                {
                    if (conversionType.IsEnum)
                    {
                        return Enum.ToObject(conversionType, Convert.ChangeType(value, Enum.GetUnderlyingType(conversionType)));
                    }
                    else
                    {
                        if (TType<bool>.Type == conversionType)
                            return int.Equals(1, Convert.ChangeType(value, TypeCode.Int32));
                        if (TType<Guid>.Type == conversionType)
                            return (Guid)value;
                        if (TType<Money>.Type == conversionType)
                            return (Money)(decimal)value;
                        if (TType<byte[]>.Type == conversionType)
                            return (byte[])value;
                        return Convert.ChangeType(value, conversionType);
                    }
                }
            }
            return null;
        }
    }
}
