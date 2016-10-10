using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

using Cnaws.Templates;

namespace Cnaws
{
    public static class Types
    {
        private static readonly Type[] _types;
        public static readonly Type TListType;
        private static readonly Type _iListType;
        private static readonly Type _iDictionaryType;
        private static readonly Type _itListType;
        private static readonly Type _itDictionaryType;

        private static readonly object[] _values;

        static Types()
        {
            _types = new Type[19];
            _values = new object[19];
            _types[0] = null;
            _values[0] = null;
            _types[1] = TType<object>.Type;
            _values[1] = null;
            _types[2] = TType<DBNull>.Type;
            _values[2] = null;
            _types[3] = TType<bool>.Type;
            _values[3] = false;
            _types[4] = TType<char>.Type;
            _values[4] = '\0';
            _types[5] = TType<sbyte>.Type;
            _values[5] = (sbyte)0;
            _types[6] = TType<byte>.Type;
            _values[6] = (byte)0;
            _types[7] = TType<short>.Type;
            _values[7] = (short)0;
            _types[8] = TType<ushort>.Type;
            _values[8] = (ushort)0;
            _types[9] = TType<int>.Type;
            _values[9] = 0;
            _types[10] = TType<uint>.Type;
            _values[10] = 0U;
            _types[11] = TType<long>.Type;
            _values[11] = 0L;
            _types[12] = TType<ulong>.Type;
            _values[12] = 0UL;
            _types[13] = TType<float>.Type;
            _values[13] = 0.0F;
            _types[14] = TType<double>.Type;
            _values[14] = 0.0;
            _types[15] = TType<decimal>.Type;
            _values[15] = 0M;
            _types[16] = TType<DateTime>.Type;
            _values[16] = new DateTime(1970, 1, 1);
            _types[17] = TType<object>.Type;
            _values[17] = null;
            _types[18] = TType<string>.Type;
            _values[18] = null;
            TListType = typeof(List<>);
            _iListType = typeof(IList);
            _iDictionaryType = typeof(IDictionary);
            _itListType = typeof(IList<>);
            _itDictionaryType = typeof(IDictionary<,>);
        }

        public static Type Get(TypeCode code)
        {
            return _types[(int)code];
        }
        public static object GetDefaultValue(TypeCode code)
        {
            return _values[(int)code];
        }
        public static object GetDefaultValue(Type type)
        {
            return GetDefaultValue(Type.GetTypeCode(type));
        }
        public static bool Equals(Type type, TypeCode code)
        {
            return _types[(int)code] == type;
        }
        private static bool IsAssignable(Type type1, Type type2)
        {
            if (type1 == type2) return true;
            if (type1.IsAssignableFrom(type2)) return true;
            if (type1.IsGenericType && type2.IsGenericType)
                return type1.GetGenericTypeDefinition() == type2.GetGenericTypeDefinition();
            return false;
        }
        public static bool IsIList(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return IsAssignable(_iListType, type);
        }
        public static bool IsIList(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Type type = value.GetType();
            return IsAssignable(_iListType, type);
        }
        public static bool IsITList(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return IsAssignable(_itListType, type);
        }
        public static bool IsITList(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Type type = value.GetType();
            return IsAssignable(_itListType, type);
        }
        public static bool IsIDictionary(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return IsAssignable(_iDictionaryType, type);
        }
        public static bool IsIDictionary(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Type type = value.GetType();
            return IsAssignable(_iDictionaryType, type);
        }
        public static bool IsITDictionary(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return IsAssignable(_itDictionaryType, type);
        }
        public static bool IsITDictionary(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Type type = value.GetType();
            return IsAssignable(_itDictionaryType, type);
        }

        public static bool GetBooleanFromString(string s)
        {
            if ("True".Equals(s)) return true;
            if ("true".Equals(s)) return true;
            if ("on".Equals(s)) return true;
            if ("1".Equals(s)) return true;
            return false;
        }
        public static object GetObjectFromString(this Type type, string s)
        {
            if (s == null)
                return null;

            if (type.IsEnum)
                return Enum.Parse(type, s);

            if (type.IsArray)
            {
                Type itemType = type.GetElementType();
                if (s.Length == 0) return Array.CreateInstance(itemType, 0);
                string[] array = s.Split(',');
                Array r = System.Array.CreateInstance(itemType, array.Length);
                for (int i = 0; i < array.Length; ++i)
                    r.SetValue(GetObjectFromString(itemType, array[i].Trim()), i);
                return r;
            }

            if (type == TType<bool>.Type) return GetBooleanFromString(s);
            if (type == TType<byte>.Type) return byte.Parse(s);
            if (type == TType<char>.Type) return char.Parse(s);
            if (type == TType<DateTime>.Type) return DateTime.Parse(s);
            if (type == TType<decimal>.Type) return decimal.Parse(s);
            if (type == TType<double>.Type) return double.Parse(s);
            if (type == TType<short>.Type) return short.Parse(s);
            if (type == TType<int>.Type) return int.Parse(s);
            if (type == TType<long>.Type) return long.Parse(s);
            if (type == TType<sbyte>.Type) return sbyte.Parse(s);
            if (type == TType<float>.Type) return float.Parse(s);
            if (type == TType<string>.Type) return s;
            if (type == TType<ushort>.Type) return ushort.Parse(s);
            if (type == TType<uint>.Type) return uint.Parse(s);
            if (type == TType<ulong>.Type) return ulong.Parse(s);
            if (type == TType<Guid>.Type) return (new Guid(s));
            if (type == TType<Money>.Type) return Money.Parse(s);
            if (type == TType<TimeSpan>.Type) return TimeSpan.Parse(s);

            if (type == TType<Uri>.Type) return (new Uri(s));
            if (type == TType<Type>.Type) return Type.GetType(s, true);
            if (type == TType<Version>.Type) return (new Version(s));
            if (type == TType<IntPtr>.Type) return (new IntPtr(int.Parse(s)));
            if (type == TType<UIntPtr>.Type) return (new UIntPtr(uint.Parse(s)));

            return TypeDescriptor.GetConverter(type).ConvertFrom(s);
        }
    }
}
