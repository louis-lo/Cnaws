using Cnaws.Templates;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cnaws
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Money : IComparable, IFormattable, IConvertible, IComparable<Money>, IEquatable<Money>
    {
        private const decimal MAXVALUE = 922337203685477.5807M;
        private const decimal MINVALUE = -922337203685477.5808M;

        public static readonly Money MaxValue;
        public static readonly Money Zero;
        public static readonly Money MinValue;

        private decimal m_value;

        static Money()
        {
            MaxValue = (Money)MAXVALUE;
            Zero = 0;
            MinValue = (Money)MINVALUE;
        }
        public Money(decimal value)
        {
            if (value > MAXVALUE || value < MINVALUE)
                throw new ArgumentOutOfRangeException("value");
            m_value = value;
        }
        public Money(int value)
        {
            m_value = value;
        }
        [CLSCompliant(false)]
        public Money(uint value)
        {
            m_value = value;
        }
        public Money(long value)
        {
            if (value > MAXVALUE || value < MINVALUE)
                throw new ArgumentOutOfRangeException("value");
            m_value = value;
        }
        [CLSCompliant(false)]
        public Money(ulong value)
        {
            if (value > MAXVALUE)
                throw new ArgumentOutOfRangeException("value");
            m_value = value;
        }
        public Money(float value)
            : this(new decimal(value))
        {
        }
        public Money(double value)
             : this(new decimal(value))
        {
        }
        
        public static Money Add(Money d1, Money d2)
        {
            return (Money)decimal.Add(d1, d2);
        }
        public static Money Ceiling(Money d)
        {
            return (Money)decimal.Ceiling(d);
        }
        public static int Compare(Money d1, Money d2)
        {
            return decimal.Compare(d1, d2);
        }
        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (!(value is Money))
                throw new ArgumentException();
            return Compare(this, (Money)value);
        }
        public int CompareTo(Money value)
        {
            return Compare(this, value);
        }
        public static Money Divide(Money d1, Money d2)
        {
            return (Money)decimal.Divide(d1, d2);
        }
        public override bool Equals(object value)
        {
            return ((value is Money) && (Compare(this, (Money)value) == 0));
        }
        public bool Equals(Money value)
        {
            return (Compare(this, value) == 0);
        }
        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }
        public static bool Equals(Money d1, Money d2)
        {
            return (Compare(d1, d2) == 0);
        }
        public static decimal Floor(Money d)
        {
            return (Money)decimal.Floor(d);
        }
        public override string ToString()
        {
            return m_value.ToString();
        }
        /// <summary>
        /// G: 16325.62
        /// C: $16,325.62
        /// E04: 1.6326E+004
        /// F: 16325.62
        /// N: 16,325.62
        /// P: 163.26 %
        /// 0,0.000: 16,325.620
        /// #,#.00#;(#,#.00#): (16,325.62)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return m_value.ToString(format);
        }
        public string ToString(IFormatProvider provider)
        {
            return m_value.ToString(provider);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return m_value.ToString(format, provider);
        }
        public static Money Parse(string s)
        {
            return (Money)decimal.Parse(s);
        }
        public static Money Parse(string s, NumberStyles style)
        {
            return (Money)decimal.Parse(s, style);
        }
        public static Money Parse(string s, IFormatProvider provider)
        {
            return (Money)decimal.Parse(s, provider);
        }
        public static Money Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            return (Money)decimal.Parse(s, style, provider);
        }
        public static bool TryParse(string s, out Money result)
        {
            decimal value;
            bool ret = decimal.TryParse(s, out value);
            result = (Money)value;
            return ret;
        }
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Money result)
        {
            decimal value;
            bool ret = decimal.TryParse(s, style, provider, out value);
            result = (Money)value;
            return ret;
        }
        public static Money Remainder(Money d1, Money d2)
        {
            return (Money)decimal.Remainder(d1, d2);
        }
        public static Money Multiply(Money d1, Money d2)
        {
            return (Money)decimal.Multiply(d1, d2);
        }
        public static Money Negate(Money d)
        {
            return (Money)decimal.Negate(d);
        }
        public static Money Round(Money d)
        {
            return (Money)decimal.Round(d);
        }
        public static Money Round(Money d, int decimals)
        {
            return (Money)decimal.Round(d, decimals);
        }
        public static Money Round(Money d, MidpointRounding mode)
        {
            return (Money)decimal.Round(d, mode);
        }
        public static Money Round(Money d, int decimals, MidpointRounding mode)
        {
            return (Money)decimal.Round(d, decimals, mode);
        }
        public static Money Subtract(Money d1, Money d2)
        {
            return (Money)decimal.Subtract(d1, d2);
        }
        public static decimal ToDecimal(Money value)
        {
            return value.m_value;
        }
        public static byte ToByte(Money value)
        {
            return decimal.ToByte(value);
        }
        [CLSCompliant(false)]
        public static sbyte ToSByte(Money value)
        {
            return decimal.ToSByte(value);
        }
        public static short ToInt16(Money value)
        {
            return decimal.ToInt16(value);
        }
        public static double ToDouble(Money d)
        {
            return decimal.ToDouble(d);
        }
        public static int ToInt32(Money d)
        {
            return decimal.ToInt32(d);
        }
        public static long ToInt64(Money d)
        {
            return decimal.ToInt64(d);
        }
        [CLSCompliant(false)]
        public static ushort ToUInt16(Money value)
        {
            return decimal.ToUInt16(value);
        }
        [CLSCompliant(false)]
        public static uint ToUInt32(Money d)
        {
            return decimal.ToUInt32(d);
        }
        [CLSCompliant(false)]
        public static ulong ToUInt64(Money d)
        {
            return decimal.ToUInt64(d);
        }
        public static float ToSingle(Money d)
        {
            return decimal.ToSingle(d);
        }
        public static Money Truncate(Money d)
        {
            return (Money)decimal.Truncate(d);
        }
        public static explicit operator Money(decimal value)
        {
            return new Money(value);
        }
        public static implicit operator Money(byte value)
        {
            return new Money(value);
        }
        [CLSCompliant(false)]
        public static implicit operator Money(sbyte value)
        {
            return new Money(value);
        }
        public static implicit operator Money(short value)
        {
            return new Money(value);
        }
        [CLSCompliant(false)]
        public static implicit operator Money(ushort value)
        {
            return new Money(value);
        }
        public static implicit operator Money(char value)
        {
            return new Money(value);
        }
        public static implicit operator Money(int value)
        {
            return new Money(value);
        }
        [CLSCompliant(false)]
        public static implicit operator Money(uint value)
        {
            return new Money(value);
        }
        public static explicit operator Money(long value)
        {
            return new Money(value);
        }
        [CLSCompliant(false)]
        public static explicit operator Money(ulong value)
        {
            return new Money(value);
        }
        public static explicit operator Money(float value)
        {
            return new Money(value);
        }
        public static explicit operator Money(double value)
        {
            return new Money(value);
        }
        public static implicit operator decimal(Money value)
        {
            return ToDecimal(value);
        }
        public static explicit operator byte(Money value)
        {
            return ToByte(value);
        }
        [CLSCompliant(false)]
        public static explicit operator sbyte(Money value)
        {
            return ToSByte(value);
        }
        public static explicit operator char(Money value)
        {
            return (char)ToUInt16(value);
        }
        public static explicit operator short(Money value)
        {
            return ToInt16(value);
        }
        [CLSCompliant(false)]
        public static explicit operator ushort(Money value)
        {
            return ToUInt16(value);
        }
        public static explicit operator int(Money value)
        {
            return ToInt32(value);
        }
        [CLSCompliant(false)]
        public static explicit operator uint(Money value)
        {
            return ToUInt32(value);
        }
        public static explicit operator long(Money value)
        {
            return ToInt64(value);
        }
        [CLSCompliant(false)]
        public static explicit operator ulong(Money value)
        {
            return ToUInt64(value);
        }
        public static explicit operator float(Money value)
        {
            return ToSingle(value);
        }
        public static explicit operator double(Money value)
        {
            return ToDouble(value);
        }
        public static Money operator +(Money d)
        {
            return d;
        }
        public static Money operator -(Money d)
        {
            return Negate(d);
        }
        public static Money operator ++(Money d)
        {
            return Add(d, 1);
        }
        public static Money operator --(Money d)
        {
            return Subtract(d, 1);
        }
        public static Money operator +(Money d1, Money d2)
        {
            return Add(d1, d2);
        }
        public static Money operator -(Money d1, Money d2)
        {
            return Subtract(d1, d2);
        }
        public static Money operator *(Money d1, Money d2)
        {
            return Multiply(d1, d2);
        }
        public static Money operator /(Money d1, Money d2)
        {
            return Divide(d1, d2);
        }
        public static Money operator %(Money d1, Money d2)
        {
            return Remainder(d1, d2);
        }
        public static bool operator ==(Money d1, Money d2)
        {
            return (Compare(d1, d2) == 0);
        }
        public static bool operator !=(Money d1, Money d2)
        {
            return (Compare(d1, d2) != 0);
        }
        public static bool operator <(Money d1, Money d2)
        {
            return (Compare(d1, d2) < 0);
        }
        public static bool operator <=(Money d1, Money d2)
        {
            return (Compare(d1, d2) <= 0);
        }
        public static bool operator >(Money d1, Money d2)
        {
            return (Compare(d1, d2) > 0);
        }
        public static bool operator >=(Money d1, Money d2)
        {
            return (Compare(d1, d2) >= 0);
        }
        public TypeCode GetTypeCode()
        {
            return TypeCode.Decimal;
        }
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(m_value);
        }
        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(m_value);
        }
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(m_value);
        }
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(m_value);
        }
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(m_value);
        }
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(m_value);
        }
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(m_value);
        }
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(m_value);
        }
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(m_value);
        }
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(m_value);
        }
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(m_value);
        }
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return m_value;
        }
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type == TType<Money>.Type) return this;
            IConvertible value = this;
            if (type == TType<bool>.Type) return value.ToBoolean(provider);
            if (type == TType<char>.Type) return value.ToChar(provider);
            if (type == TType<sbyte>.Type) return value.ToSByte(provider);
            if (type == TType<byte>.Type) return value.ToByte(provider);
            if (type == TType<short>.Type) return value.ToInt16(provider);
            if (type == TType<ushort>.Type) return value.ToUInt16(provider);
            if (type == TType<int>.Type) return value.ToInt32(provider);
            if (type == TType<uint>.Type) return value.ToUInt32(provider);
            if (type == TType<long>.Type) return value.ToInt64(provider);
            if (type == TType<ulong>.Type) return value.ToUInt64(provider);
            if (type == TType<float>.Type) return value.ToSingle(provider);
            if (type == TType<double>.Type) return value.ToDouble(provider);
            if (type == TType<decimal>.Type) return value.ToDecimal(provider);
            if (type == TType<DateTime>.Type) return value.ToDateTime(provider);
            if (type == TType<string>.Type) return value.ToString(provider);
            if (type == TType<object>.Type) return value;
            if (type == TType<Enum>.Type) return (Enum)value;
            //if (type == TType<DBNull>.Type) throw new InvalidCastException();
            //if (type == TType<Empty>.Type) throw new InvalidCastException();
            throw new InvalidCastException();
        }
    }

    public static class MoneyExtensions
    {
        public static Money Abs(this Money value)
        {
            return new Money(Math.Abs(value));
        }
    }
}
