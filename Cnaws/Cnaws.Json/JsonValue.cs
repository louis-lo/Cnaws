using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Dynamic;

namespace Cnaws.Json
{
    public enum JsonValueType
    {
        Null,
        Boolean,
        Number,
        String,
        Array,
        Object
    }

    public enum JsonNumberType
    {
        SByte,
        Byte,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        Decimal
    }

    public abstract class JsonValue
    {
        public static readonly JsonNull Null = new JsonNull();
        public static readonly JsonBoolean True = new JsonBoolean(true);
        public static readonly JsonBoolean False = new JsonBoolean(false);

        protected JsonValue()
        {
        }

        public abstract JsonValueType Type { get; }

        public void Render(TextWriter writer)
        {
            writer.Write(ToJsonString());
        }
        public abstract string ToJsonString();
        public override string ToString()
        {
            return ToJsonString();
        }

        protected static JsonValue Load(object value)
        {
            if (value == null)
                return Null;
            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.Boolean: return ((bool)value) ? True : False;
                case TypeCode.SByte: return new JsonNumber((sbyte)value);
                case TypeCode.Byte: return new JsonNumber((byte)value);
                case TypeCode.Int16: return new JsonNumber((short)value);
                case TypeCode.UInt16: return new JsonNumber((ushort)value);
                case TypeCode.Int32: return new JsonNumber((int)value);
                case TypeCode.UInt32: return new JsonNumber((uint)value);
                case TypeCode.Int64: return new JsonNumber((long)value);
                case TypeCode.UInt64: return new JsonNumber((ulong)value);
                case TypeCode.Single: return new JsonNumber((float)value);
                case TypeCode.Double: return new JsonNumber((double)value);
                case TypeCode.Decimal: return new JsonNumber((decimal)value);
                case TypeCode.Char: return new JsonString((char)value);
                case TypeCode.String: return new JsonString((string)value);
                case TypeCode.DateTime: return new JsonNumber(((DateTime)value).ToTimestamp());
            }
            Type type = value.GetType();
            //if (TType<Type>.Type == type)
            //    return new JsonObject(System.Type.GetType((string)value));
            if (type.IsEnum)
                return new JsonString(value.ToString());
            if (type.IsArray)
                return new JsonArray((Array)value);

            if (TType<Guid>.Type == type)
                return new JsonString(((Guid)value).ToString());
            if (TType<Money>.Type == type)
                return new JsonNumber((Money)value);

            TypesInfo info = new TypesInfo(type);
            if (info.IsIDict)
            {
                Type[] types;
                if (info.IsItDict)
                    types = info.TDict;
                else
                    types = type.GetGenericArguments();
                if (types.Length == 2 && Types.Equals(types[0], TypeCode.String))
                    return new JsonObject((IDictionary)value);
            }
            if (info.IsIList)
                return new JsonArray((IList)value);
            return new JsonObject(value);
        }
        public static JsonValue LoadJson(string json)
        {
            return (new JsonReader(json)).ReadValue();
        }
        //public static string ToJson(object o)
        //{
        //    if (o != null && TType<DynamicObject>.Type.IsAssignableFrom(o.GetType()))
        //        return (new JsonWriterEx(o)).WriteValue();
        //    return (new JsonWriter(o)).WriteValue();
        //}
        public static string Serialize<T>(T o)
        {
            return (new JsonWriter<T>(o)).WriteValue();
        }
        public static T Deserialize<T>(string json)
        {
            return (new JsonReader<T>(json)).ReadValue();
        }

        //public static string PropertiesToJson(object o)
        //{
        //    return (new JsonWriterEx(o)).WriteValue();
        //}
        //public static string SerializeProperties<T>(T o)
        //{
        //    return (new JsonWriterEx<T>(o)).WriteValue();
        //}
    }

    public sealed class JsonNull : JsonValue
    {
        internal JsonNull()
        {
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.Null; }
        }

        public override string ToJsonString()
        {
            return "null";
        }
    }

    public sealed class JsonBoolean : JsonValue
    {
        private bool _value;

        internal JsonBoolean(bool value)
        {
            _value = value;
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.Boolean; }
        }
        public bool Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToJsonString()
        {
            return _value ? "true" : "false";
        }

        public static implicit operator bool(JsonBoolean value)
        {
            return value.Value;
        }
        public static implicit operator JsonBoolean(bool value)
        {
            return value ? JsonValue.True : JsonValue.False;
        }
    }

    public sealed class JsonObject : JsonValue, IEnumerable
    {
        private Hashtable _value;

        public JsonObject()
        {
            _value = new Hashtable();
        }
        internal JsonObject(IDictionary value)
        {
            _value = new Hashtable();

            foreach (string key in value.Keys)
                this[key] = JsonValue.Load(value[key]);
        }
        internal JsonObject(object value)
        {
            _value = new Hashtable();

            KeyValuePair<string, FieldInfo> field;
            List<KeyValuePair<string, FieldInfo>> fields = value.GetType().GetStaticAllNameGetFields<JsonFieldAttribute>();
            for (int i = 0; i < fields.Count; ++i)
            {
                field = fields[i];
                this[field.Key] = JsonValue.Load(field.Value.GetValue(value));
            }
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.Object; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="System.ArgumentNullException">key 为 null。</exception>
        /// <returns></returns>
        public JsonValue this[string key]
        {
            get
            {
                JsonValue value = _value[key] as JsonValue;
                if (value != null)
                    return value;
                return JsonValue.Null;
            }
            set { _value[key] = value; }
        }

        public bool ContainsKey(string key)
        {
            return _value.ContainsKey(key);
        }

        public override string ToJsonString()
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            foreach (string key in _value.Keys)
            {
                if (i++ > 0)
                    sb.Append(',');
                sb.Append('"');
                sb.Append(key);
                sb.Append("\":");
                sb.Append(this[key].ToJsonString());
            }
            sb.Append('}');
            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return _value.GetEnumerator();
        }
    }

    public sealed class JsonArray : JsonValue, IEnumerable
    {
        private List<JsonValue> _value;

        public JsonArray()
        {
            _value = new List<JsonValue>();
        }
        internal JsonArray(Array value)
        {
            _value = new List<JsonValue>();

            Type type = value.GetType();
            Type elementType = type.GetElementType();
            for (int i = 0; i < value.Length; ++i)
                _value.Add(JsonValue.Load(value.GetValue(i)));
        }
        internal JsonArray(IList value)
        {
            _value = new List<JsonValue>();

            for (int i = 0; i < value.Count; ++i)
                _value.Add(JsonValue.Load(value[i]));
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.Array; }
        }

        public int Count
        {
            get { return _value.Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 - index 等于或大于 Count。</exception>
        /// <returns></returns>
        public JsonValue this[int index]
        {
            get { return _value[index]; }
            set { _value[index] = value; }
        }

        public void Add(JsonValue value)
        {
            _value.Add(value);
        }

        public override string ToJsonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < _value.Count; ++i)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(this[i].ToJsonString());
            }
            sb.Append(']');
            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return _value.GetEnumerator();
        }

        //public static implicit operator Array(JsonArray value)
        //{
        //    return value.Value;
        //}
        //public static implicit operator JsonArray(Array value)
        //{
        //    return new JsonArray(value);
        //}
    }

    public sealed class JsonNumber : JsonValue
    {
        private abstract class Number
        {
            public abstract JsonNumberType Type { get; }
            public abstract object Value { get; }
        }
        private sealed class SByteNumber : Number
        {
            private sbyte _value;

            public SByteNumber(sbyte value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.SByte; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class ByteNumber : Number
        {
            private byte _value;

            public ByteNumber(byte value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Byte; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class Int16Number : Number
        {
            private short _value;

            public Int16Number(short value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Int16; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class Int32Number : Number
        {
            private int _value;

            public Int32Number(int value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Int32; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class Int64Number : Number
        {
            private long _value;

            public Int64Number(long value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Int64; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class UInt16Number : Number
        {
            private ushort _value;

            public UInt16Number(ushort value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.UInt16; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class UInt32Number : Number
        {
            private uint _value;

            public UInt32Number(uint value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.UInt32; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class UInt64Number : Number
        {
            private ulong _value;

            public UInt64Number(ulong value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.UInt64; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class SingleNumber : Number
        {
            private float _value;

            public SingleNumber(float value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Single; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class DoubleNumber : Number
        {
            private double _value;

            public DoubleNumber(double value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Double; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
        private sealed class DecimalNumber : Number
        {
            private decimal _value;

            public DecimalNumber(decimal value)
            {
                _value = value;
            }

            public override JsonNumberType Type
            {
                get { return JsonNumberType.Decimal; }
            }
            public override object Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }

        private Number _value;

        public JsonNumber()
        {
            _value = new Int32Number(0);
        }
        public JsonNumber(sbyte value)
        {
            _value = new SByteNumber(value);
        }
        public JsonNumber(byte value)
        {
            _value = new ByteNumber(value);
        }
        public JsonNumber(short value)
        {
            _value = new Int16Number(value);
        }
        public JsonNumber(int value)
        {
            _value = new Int32Number(value);
        }
        public JsonNumber(long value)
        {
            _value = new Int64Number(value);
        }
        public JsonNumber(ushort value)
        {
            _value = new UInt16Number(value);
        }
        public JsonNumber(uint value)
        {
            _value = new UInt32Number(value);
        }
        public JsonNumber(ulong value)
        {
            _value = new UInt64Number(value);
        }
        public JsonNumber(float value)
        {
            _value = new SingleNumber(value);
        }
        public JsonNumber(double value)
        {
            _value = new DoubleNumber(value);
        }
        public JsonNumber(decimal value)
        {
            _value = new DecimalNumber(value);
        }
        public JsonNumber(Money value)
            : this((decimal)value)
        {
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.Number; }
        }
        public JsonNumberType NumberType
        {
            get { return _value.Type; }
        }

        public object Value
        {
            get { return _value.Value; }
        }

        public void Set(sbyte value)
        {
            _value = new SByteNumber(value);
        }
        public void Set(byte value)
        {
            _value = new ByteNumber(value);
        }
        public void Set(short value)
        {
            _value = new Int16Number(value);
        }
        public void Set(int value)
        {
            _value = new Int32Number(value);
        }
        public void Set(long value)
        {
            _value = new Int64Number(value);
        }
        public void Set(ushort value)
        {
            _value = new UInt16Number(value);
        }
        public void Set(uint value)
        {
            _value = new UInt32Number(value);
        }
        public void Set(ulong value)
        {
            _value = new UInt64Number(value);
        }
        public void Set(float value)
        {
            _value = new SingleNumber(value);
        }
        public void Set(double value)
        {
            _value = new DoubleNumber(value);
        }
        public void Set(decimal value)
        {
            _value = new DecimalNumber(value);
        }

        public override string ToJsonString()
        {
            return _value.ToString();
        }

        public static explicit operator sbyte(JsonNumber value)
        {
            return (sbyte)value.Value;
        }
        public static implicit operator JsonNumber(sbyte value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator byte(JsonNumber value)
        {
            return (byte)value.Value;
        }
        public static implicit operator JsonNumber(byte value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator short(JsonNumber value)
        {
            return (short)value.Value;
        }
        public static implicit operator JsonNumber(short value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator int(JsonNumber value)
        {
            return (int)value.Value;
        }
        public static implicit operator JsonNumber(int value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator long(JsonNumber value)
        {
            return (long)value.Value;
        }
        public static implicit operator JsonNumber(long value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator ushort(JsonNumber value)
        {
            return (ushort)value.Value;
        }
        public static implicit operator JsonNumber(ushort value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator uint(JsonNumber value)
        {
            return (uint)value.Value;
        }
        public static implicit operator JsonNumber(uint value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator ulong(JsonNumber value)
        {
            return (ulong)value.Value;
        }
        public static implicit operator JsonNumber(ulong value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator float(JsonNumber value)
        {
            return (float)value.Value;
        }
        public static implicit operator JsonNumber(float value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator double(JsonNumber value)
        {
            return (double)value.Value;
        }
        public static implicit operator JsonNumber(double value)
        {
            return new JsonNumber(value);
        }
        public static explicit operator decimal(JsonNumber value)
        {
            return (decimal)value.Value;
        }
        public static implicit operator JsonNumber(decimal value)
        {
            return new JsonNumber(value);
        }
    }

    public sealed class JsonString : JsonValue
    {
        private static readonly byte[] CHAR_ESCAPE = {
            0, 0, 0, 0, 0, 0, 0, 0, (byte)'b', (byte)'t', (byte)'n', 0, (byte)'f', (byte)'r', 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, (byte)'"', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,(byte)'\\', 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private string _value;

        public JsonString()
            : this(null)
        {
        }
        public JsonString(string value)
        {
            _value = value;
        }
        public JsonString(char value)
        {
            _value = value.ToString();
        }

        public override JsonValueType Type
        {
            get { return JsonValueType.String; }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToJsonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            sb.Append(ToString());
            sb.Append('"');
            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_value != null)
            {
                byte e;
                char ch;
                for (int i = 0; i < _value.Length; ++i)
                {
                    ch = _value[i];
                    if (ch < 256)
                    {
                        e = CHAR_ESCAPE[ch];
                        if (e != 0)
                        {
                            sb.Append('\\');
                            sb.Append((char)e);
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                    }
                    else
                    {
                        //stream.Write("\\u");
                        //byte[] bytes = Encoding.Unicode.GetBytes(new char[] { ch });
                        //stream.Write(Convert.ToString(bytes[1], 16));
                        //stream.Write(Convert.ToString(bytes[0], 16));
                        sb.Append(ch);
                    }
                }
            }
            return sb.ToString();
        }

        public static implicit operator string(JsonString value)
        {
            return value.Value;
        }
        public static implicit operator JsonString(string value)
        {
            return new JsonString(value);
        }
        public static implicit operator JsonString(JsonNumber value)
        {
            return new JsonString(value.Value.ToString());
        }
    }
}
