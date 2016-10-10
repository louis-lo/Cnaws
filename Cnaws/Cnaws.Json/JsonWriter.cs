using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Dynamic;

namespace Cnaws.Json
{
    internal class JsonWriter
    {
        private readonly object _value;
        private readonly Type _type;

        public JsonWriter(object obj)
        {
            _value = obj;
            _type = null;
        }
        internal JsonWriter(object obj, Type type)
        {
            _value = obj;
            _type = type;
        }

        public string WriteValue()
        {
            object value = _value;
            if (value == null || DBNull.Value.Equals(value))
                return JsonValue.Null.ToJsonString();

            Type type = value.GetType();
            if (_type != null && _type != type && !_type.IsAssignableFrom(type))
            {
                type = _type;
                value = Convert.ChangeType(value, _type);
            }

            //if (typeof(Type) == type)
            //    return (new JsonString(((Type)value).FullName)).ToJsonString();

            if (type.IsEnum)
                return (new JsonString(value.ToString())).ToJsonString();

            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.Boolean:
                    return (((bool)value) ? JsonValue.True : JsonValue.False).ToJsonString();
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return value.ToString();
                case TypeCode.Char:
                    return (new JsonString((char)value)).ToJsonString();
                case TypeCode.String:
                    return (new JsonString((string)value)).ToJsonString();
                case TypeCode.DateTime:
                    return (((DateTime)value).ToTimestamp()).ToString();
            }

            if (TType<Guid>.Type == type)
                return (new JsonString(((Guid)value).ToString())).ToJsonString();
            if (TType<Money>.Type == type)
                return value.ToString();

            StringBuilder sb = new StringBuilder();
            if (type.IsArray)
            {
                Array array = (Array)value;
                Type elementType = type.GetElementType();
                sb.Append('[');
                for (int i = 0; i < array.Length; ++i)
                {
                    if (i > 0)
                        sb.Append(',');
                    sb.Append(CreateJsonWriter(array.GetValue(i), elementType).WriteValue());
                }
                sb.Append(']');
                return sb.ToString();
            }

            TypesInfo info = new TypesInfo(type);
            if (info.IsIDict)
            {
                IDictionary dict = (IDictionary)value;
                Type[] types;
                if (info.IsItDict)
                    types = info.TDict;
                else
                    types = type.GetGenericArguments();
                if (types.Length == 2 && Types.Equals(types[0], TypeCode.String))
                {
                    int i = 0;
                    Type elementType = types[1];
                    sb.Append('{');
                    foreach (object key in dict.Keys)
                    {
                        if (key != null && key is string)
                        {
                            if (i++ > 0)
                                sb.Append(',');
                            sb.Append('"');
                            sb.Append(key);
                            sb.Append("\":");
                            sb.Append(CreateJsonWriter(dict[key], elementType).WriteValue());
                        }
                    }
                    sb.Append('}');
                    return sb.ToString();
                }
            }

            if (info.IsIList)
            {
                IList list = (IList)value;
                Type[] types;
                if (info.IsItList)
                    types = info.TList;
                else
                    types = type.GetGenericArguments();
                Type elementType = null;
                if (types.Length == 1)
                    elementType = types[0];
                sb.Append('[');
                for (int i = 0; i < list.Count; ++i)
                {
                    if (i > 0)
                        sb.Append(',');
                    sb.Append(CreateJsonWriter(list[i], elementType).WriteValue());
                }
                sb.Append(']');
                return sb.ToString();
            }

            sb.Append('{');
            WriteObject(value, type, sb);
            sb.Append('}');
            return sb.ToString();
        }
        protected virtual JsonWriter CreateJsonWriter(object obj, Type type)
        {
            if (obj != null)
                type = obj.GetType();
            if (type.IsAnonymousType())
                return new JsonWriterEx(obj, type);
            return new JsonWriter(obj, type);
        }
        protected virtual void WriteObject(object value, Type type, StringBuilder sb)
        {
            KeyValuePair<string, FieldInfo> member;
            List<KeyValuePair<string, FieldInfo>> members = type.GetStaticAllNameGetFields<JsonFieldAttribute>();
            for (int i = 0; i < members.Count; ++i)
            {
                if (i > 0)
                    sb.Append(',');
                member = members[i];
                sb.Append('"');
                sb.Append(member.Key);
                sb.Append("\":");
                sb.Append(CreateJsonWriter(member.Value.GetValue(value), member.Value.FieldType).WriteValue());
            }
        }
    }
    internal class JsonWriter<T>
    {
        private readonly T _value;

        public JsonWriter(T obj)
        {
            _value = obj;
        }

        protected T Value
        {
            get { return _value; }
        }

        public virtual string WriteValue()
        {
            if (TType<T>.Type.IsAnonymousType())
            {
                if (Value == null)
                    return (new JsonWriterEx(default(T))).WriteValue();
                return (new JsonWriterEx(Value, TType<T>.Type)).WriteValue();
            }
            else {
                if (_value == null)
                    return (new JsonWriter(default(T))).WriteValue();
                return (new JsonWriter(_value, TType<T>.Type)).WriteValue();
            }
        }
    }
    internal sealed class JsonWriterEx : JsonWriter
    {
        public JsonWriterEx(object obj)
            : base(obj)
        {
        }
        internal JsonWriterEx(object obj, Type type)
            : base(obj, type)
        {
        }
        
        protected override void WriteObject(object value, Type type, StringBuilder sb)
        {
            int i = 0;
            Dictionary<string, PropertyInfo> members = type.GetStaticProperties();
            foreach (KeyValuePair<string, PropertyInfo> member in members)
            {
                if (i++ > 0)
                    sb.Append(',');
                sb.Append('"');
                sb.Append(member.Key);
                sb.Append("\":");
                sb.Append(CreateJsonWriter(member.Value.GetValue(value), member.Value.PropertyType).WriteValue());
            }
        }
    }
}
