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

namespace Cnaws.Json
{
    internal abstract class JsonReaderBase
    {
        protected const int HEX_BUFF_LENGTH = 4;
        protected static readonly byte[] CHAR_ESCAPE ={
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0,(byte)'\"', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,(byte)'/', 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,(byte)'\\', 0, 0, 0, 
			0, 0,(byte)'\b', 0, 0, 0,(byte)'\f', 0, 0, 0, 0, 0, 0, 0,(byte)'\n', 0, 
			0, 0,(byte)'\r', 0,(byte)'\t', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private string _s;
        private int _length;
        private int _pos;

        protected JsonReaderBase(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            _s = s;
            _length = _s.Length;
            _pos = 0;
        }
        ~JsonReaderBase()
        {
            _s = null;
            _length = 0;
            _pos = 0;
        }

        protected int Peek()
        {
            if (_pos == _length)
                return -1;
            return _s[_pos];
        }
        protected int Read()
        {
            if (_pos == _length)
                return -1;
            return _s[_pos++];
        }
        protected int Read([In, Out] char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if ((buffer.Length - index) < count)
                throw new ArgumentException();
            int num = _length - _pos;
            if (num > 0)
            {
                if (num > count)
                    num = count;
                _s.CopyTo(_pos, buffer, index, num);
                _pos += num;
            }
            return num;
        }
        protected string ReadToEnd()
        {
            string str;
            if (_pos == 0)
                str = _s;
            else
                str = _s.Substring(_pos, _length - _pos);
            _pos = _length;
            return str;
        }

        protected void SkipWhitespace()
        {
            while (Peek() != -1 && char.IsWhiteSpace((char)Peek()))
                Read();
        }
    }

    /// <summary>
    /// 表示提供对 JSON 数据进行快速、非缓存、只进访问的读取器。
    /// </summary>
    internal class JsonReader : JsonReaderBase
    {
        /// <summary>
        /// 为指定的字符串初始化 Cnaws.Json.JsonReader 类的新实例。
        /// </summary>
        /// <exception cref="System.ArgumentNullException">input 为 null。</exception>
        /// <param name="input">包含 JSON 数据的字符串。</param>
        public JsonReader(string input)
            : base(input)
        {
        }

        private JsonObject ReadObject()
        {
            Debug.Assert(Peek() == '{');
            Read(); // Skip '{'
            SkipWhitespace();
            JsonObject value = new JsonObject();
            if (Peek() == '}')
            {
                Read();
                return value;
            }
            JsonString key;
            for (int memberCount = 0; ; )
            {
                if (Peek() != '"')
                    throw new JsonException("Name of an object member must be a string.", ReadToEnd());
                key = ReadString();
                SkipWhitespace();
                if (Peek() != ':')
                    throw new JsonException("There must be a colon after the name of object member.", ReadToEnd());
                Read(); // Skip ':'
                SkipWhitespace();
                value[key] = ReadValue();
                SkipWhitespace();
                ++memberCount;
                switch (Read())
                {
                    case ',': SkipWhitespace(); break;
                    case '}': return value;
                    default: throw new JsonException("Must be a comma or '}' after an object member.", ReadToEnd());
                }
            }
        }
        private JsonArray ReadArray()
        {
            Debug.Assert(Peek() == '[');
            Read(); // Skip '['
            SkipWhitespace();
            JsonArray value = new JsonArray();
            if (Peek() == ']')
            {
                Read(); // Skip ']'
                return value;
            }
            for (; ; )
            {
                value.Add(ReadValue());
                SkipWhitespace();
                switch (Read())
                {
                    case ',': SkipWhitespace(); break;
                    case ']': return value;
                    default: throw new JsonException("Must be a comma or ']' after an array element.", ReadToEnd());
                }
            }
        }
        private JsonNull ReadNull()
        {
            Debug.Assert(Peek() == 'n');
            Read();
            if (Read() == 'u' && Read() == 'l' && Read() == 'l')
                return JsonValue.Null;
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        private JsonBoolean ReadTrue()
        {
            Debug.Assert(Peek() == 't');
            Read();
            if (Read() == 'r' && Read() == 'u' && Read() == 'e')
                return JsonValue.True;
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        private JsonBoolean ReadFalse()
        {
            Debug.Assert(Peek() == 'f');
            Read();
            if (Read() == 'a' && Read() == 'l' && Read() == 's' && Read() == 'e')
                return JsonValue.False;
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private JsonString ReadString()
        {
            Debug.Assert(Peek() == '\"');
            Read(); // Skip '\"'
            int c, e;
            byte f;
            StringBuilder sb = new StringBuilder();
            for (; ; )
            {
                c = Read();
                if (c == '\\') // Escape
                {
                    e = Read();
                    if (e < 256 && (f = CHAR_ESCAPE[e]) != 0)
                    {
                        sb.Append((char)f);
                    }
                    else if (e == 'u')
                    {
                        char[] buff = new char[HEX_BUFF_LENGTH];
                        if (Read(buff, 0, HEX_BUFF_LENGTH) != HEX_BUFF_LENGTH)
                            throw new JsonException("Incorrect hex digit after \\u escape.", ReadToEnd());
                        try
                        {
                            byte byteAfter = Convert.ToByte(new string(buff, 0, 2), 16);
                            byte byteBefore = Convert.ToByte(new string(buff, 2, 2), 16);
                            sb.Append(Encoding.Unicode.GetString(new byte[] { byteBefore, byteAfter }));
                        }
                        catch
                        {
                            throw new JsonException("Incorrect hex digit after \\u escape.", ReadToEnd());
                        }
                    }
                    else
                    {
                        throw new JsonException("Unknown escape character.", ReadToEnd());
                    }
                }
                else if (c == '"')
                {
                    return new JsonString(sb.ToString());
                }
                else if (c == -1)
                {
                    throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
                }
                else if (c < 0x20)
                {
                    throw new JsonException("Incorrect unescaped character in string.", ReadToEnd());
                }
                else
                {
                    sb.Append((char)c);
                }
            }
        }
        private JsonNumber ReadNumber()
        {
            NumberStyles style = NumberStyles.None;
            StringBuilder sb = new StringBuilder(1);
            if (Peek() == '-')
            {
                style |= NumberStyles.AllowLeadingSign;
                sb.Append((char)Read());
            }
            if (Peek() >= '0' && Peek() <= '9')
            {
                sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }
            if (Peek() == '.')
            {
                style |= NumberStyles.AllowDecimalPoint;
                sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }
            if (Peek() == 'e' || Peek() == 'E')
            {
                style |= NumberStyles.AllowExponent;
                sb.Append((char)Read());
                if (Peek() == '+' || Peek() == '-')
                    sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }
            string s = sb.ToString();
            if ((style & NumberStyles.AllowDecimalPoint) > 0)
                return new JsonNumber(double.Parse(s, style, NumberFormatInfo.CurrentInfo));
            decimal value = decimal.Parse(s, style, NumberFormatInfo.CurrentInfo);
            if (value >= int.MinValue && value <= int.MaxValue)
                return new JsonNumber((int)value);
            if (value >= long.MinValue && value <= long.MaxValue)
                return new JsonNumber((long)value);
            return new JsonNumber(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        /// <exception cref="Cnaws.Json.JsonException"></exception>
        /// <returns></returns>
        public JsonValue ReadValue()
        {
            int ch = Peek();
            if (ch == -1)
                throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
            switch (ch)
            {
                case 'n': return ReadNull();
                case 't': return ReadTrue();
                case 'f': return ReadFalse();
                case '"': return ReadString();
                case '{': return ReadObject();
                case '[': return ReadArray();
                default: return ReadNumber();
            }
        }
    }

    internal sealed class JsonReader<T> : JsonReader
    {
        public JsonReader(string input)
            : base(input)
        {
        }

        private bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || Types.Equals(type, TypeCode.String) || Types.Equals(type, TypeCode.DateTime);
        }
        private void ReadObject(object instance, Dictionary<string, FieldInfo> members)
        {
            Debug.Assert(Peek() == '{');
            Read(); // Skip '{'
            SkipWhitespace();
            if (Peek() == '}')
            {
                Read();
                return;
            }
            FieldInfo field;
            for (int memberCount = 0; ; )
            {
                if (Peek() != '"')
                    throw new JsonException("Name of an object member must be a string.", ReadToEnd());
                members.TryGetValue(ReadString(), out field);
                //if (!members.TryGetValue(ReadString(), out field))
                //    throw new JsonException("There must be a colon after the name of object member.", ReadToEnd());
                SkipWhitespace();
                if (Peek() != ':')
                    throw new JsonException("There must be a colon after the name of object member.", ReadToEnd());
                Read(); // Skip ':'
                SkipWhitespace();
                if (field != null)
                {
                    object value;
                    if (IsPrimitive(field.FieldType))
                        value = ReadPrimitiveValue(field.FieldType);
                    else
                        value = ReadObjectValue(field.FieldType, field.FieldType.GetStaticAllNameSetFields<JsonFieldAttribute>());
                    if (value == null)
                        field.SetValue(instance, null);
                    else
                        field.SetValue(instance, Convert.ChangeType(value, field.FieldType));
                }
                else
                {
                    base.ReadValue();
                }
                SkipWhitespace();
                ++memberCount;
                switch (Read())
                {
                    case ',': SkipWhitespace(); break;
                    case '}': return;
                    default: throw new JsonException("Must be a comma or '}' after an object member.", ReadToEnd());
                }
            }
        }
        private void ReadDictionary(Type elementType, IDictionary dict)
        {
            Debug.Assert(Peek() == '{');
            Read(); // Skip '{'
            SkipWhitespace();
            if (Peek() == '}')
            {
                Read();
                return;
            }
            string key;
            for (int memberCount = 0; ; )
            {
                if (Peek() != '"')
                    throw new JsonException("Name of an object member must be a string.", ReadToEnd());
                key = ReadString();
                SkipWhitespace();
                if (Peek() != ':')
                    throw new JsonException("There must be a colon after the name of object member.", ReadToEnd());
                Read(); // Skip ':'
                SkipWhitespace();
                object value;
                if (IsPrimitive(elementType))
                    value = ReadPrimitiveValue(elementType);
                else
                    value = ReadObjectValue(elementType, elementType.GetStaticAllNameSetFields<JsonFieldAttribute>());
                dict.Add(key, value);
                SkipWhitespace();
                ++memberCount;
                switch (Read())
                {
                    case ',': SkipWhitespace(); break;
                    case '}': return;
                    default: throw new JsonException("Must be a comma or '}' after an object member.", ReadToEnd());
                }
            }
        }
        private void ReadArray(Type elementType, IList list)
        {
            Debug.Assert(Peek() == '[');
            Read(); // Skip '['
            SkipWhitespace();
            if (Peek() == ']')
            {
                Read(); // Skip ']'
                return;
            }
            for (; ; )
            {
                object value;
                if (IsPrimitive(elementType))
                    value = ReadPrimitiveValue(elementType);
                else
                    value = ReadObjectValue(elementType, elementType.GetStaticAllNameSetFields<JsonFieldAttribute>());
                list.Add(Convert.ChangeType(value, elementType));

                SkipWhitespace();
                switch (Read())
                {
                    case ',': SkipWhitespace(); break;
                    case ']': return;
                    default: throw new JsonException("Must be a comma or ']' after an array element.", ReadToEnd());
                }
            }
        }
        private object ReadNull(Type type)
        {
            Debug.Assert(Peek() == 'n');
            Read();
            if (Read() == 'u' && Read() == 'l' && Read() == 'l')
                return type.GetDefaultValue();
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        private bool ReadTrue()
        {
            Debug.Assert(Peek() == 't');
            Read();
            if (Read() == 'r' && Read() == 'u' && Read() == 'e')
                return true;
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        private bool ReadFalse()
        {
            Debug.Assert(Peek() == 'f');
            Read();
            if (Read() == 'a' && Read() == 'l' && Read() == 's' && Read() == 'e')
                return false;
            throw new JsonException("Invalid value.", ReadToEnd());
        }
        private string ReadString()
        {
            Debug.Assert(Peek() == '\"');
            Read(); // Skip '\"'
            int c, e;
            byte f;
            StringBuilder sb = new StringBuilder();
            for (; ; )
            {
                c = Read();
                if (c == '\\') // Escape
                {
                    e = Read();
                    if (e < 256 && (f = CHAR_ESCAPE[e]) != 0)
                    {
                        sb.Append((char)f);
                    }
                    else if (e == 'u')
                    {
                        char[] buff = new char[HEX_BUFF_LENGTH];
                        if (Read(buff, 0, HEX_BUFF_LENGTH) != HEX_BUFF_LENGTH)
                            throw new JsonException("Incorrect hex digit after \\u escape.", ReadToEnd());
                        try
                        {
                            byte byteAfter = Convert.ToByte(new string(buff, 0, 2), 16);
                            byte byteBefore = Convert.ToByte(new string(buff, 2, 2), 16);
                            sb.Append(Encoding.Unicode.GetString(new byte[] { byteBefore, byteAfter }));
                        }
                        catch
                        {
                            throw new JsonException("Incorrect hex digit after \\u escape.", ReadToEnd());
                        }
                    }
                    else
                    {
                        throw new JsonException("Unknown escape character.", ReadToEnd());
                    }
                }
                else if (c == '"')
                {
                    return sb.ToString();
                }
                else if (c == -1)
                {
                    throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
                }
                else if (c < 0x20)
                {
                    throw new JsonException("Incorrect unescaped character in string.", ReadToEnd());
                }
                else
                {
                    sb.Append((char)c);
                }
            }
        }
        private Guid ReadGuid()
        {
            return Guid.Parse(ReadString());
        }
        private object ReadNumber(Type type)
        {
            NumberStyles style = NumberStyles.None;
            StringBuilder sb = new StringBuilder(1);
            if (Peek() == '-')
            {
                style |= NumberStyles.AllowLeadingSign;
                sb.Append((char)Read());
            }
            if (Peek() >= '0' && Peek() <= '9')
            {
                sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }
            if (Peek() == '.')
            {
                style |= NumberStyles.AllowDecimalPoint;
                sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }
            if (Peek() == 'e' || Peek() == 'E')
            {
                style |= NumberStyles.AllowExponent;
                sb.Append((char)Read());
                if (Peek() == '+' || Peek() == '-')
                    sb.Append((char)Read());
                while (Peek() >= '0' && Peek() <= '9')
                    sb.Append((char)Read());
            }

            if ((style & NumberStyles.AllowDecimalPoint) > 0)
            {
                if (type == TType<Money>.Type)
                    return (Money)decimal.Parse(sb.ToString(), style, NumberFormatInfo.CurrentInfo);
                return Convert.ChangeType(double.Parse(sb.ToString(), style, NumberFormatInfo.CurrentInfo), type);
            }
            if (type == TType<DateTime>.Type)
                return int.Parse(sb.ToString(), style, NumberFormatInfo.CurrentInfo).ToDateTime();
            decimal value = decimal.Parse(sb.ToString(), style, NumberFormatInfo.CurrentInfo);
            if (type == TType<Money>.Type)
                return (Money)value;
            return Convert.ChangeType(value, type);
        }
        private object ReadPrimitiveValue(Type type)
        {
            int ch = Peek();
            if (ch == -1)
                throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
            switch (ch)
            {
                case 'n':
                    return ReadNull(type);
                case 't':
                    return ReadTrue();
                case 'f':
                    return ReadFalse();
                case '"':
                    if (type == TType<Guid>.Type)
                        return ReadGuid();
                    return ReadString();
                case '{':
                    throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
                case '[':
                    throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
                default:
                    return ReadNumber(type);
            }
        }
        private object ReadObjectValue(Type type, Dictionary<string, FieldInfo> members)
        {
            int ch = Peek();
            if (ch == -1)
                throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
            switch (ch)
            {
                case 'n': return ReadNull(type);
                case 't': return ReadTrue();
                case 'f': return ReadFalse();
                case '"':
                    {
                        string s = ReadString();
                        if (type.IsEnum)
                            return Enum.Parse(type, s);
                        return s;
                    }
                case '{':
                    {
                        TypesInfo info = new TypesInfo(type);
                        if (info.IsIDict)
                        {
                            IDictionary dict = (IDictionary)Activator.CreateInstance(type);
                            Type[] types;
                            if (info.IsItDict)
                                types = info.TDict;
                            else
                                types = type.GetGenericArguments();
                            if (types.Length == 2 && Types.Equals(types[0], TypeCode.String))
                                ReadDictionary(types[1], dict);
                            else
                                ReadObject(dict, members);
                            return dict;
                        }
                        else
                        {
                            object value = Activator.CreateInstance(type);
                            ReadObject(value, members);
                            return value;
                        }
                    }
                case '[':
                    {
                        IList list;
                        Type listType;
                        Type elementType;
                        if (type.IsArray)
                        {
                            elementType = type.GetElementType();
                            listType = Types.TListType.MakeGenericType(elementType);
                            list = (IList)Activator.CreateInstance(listType);
                            ReadArray(elementType, list);
                            MethodInfo method = listType.GetMethod("ToArray", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
                            return method.Invoke(list, null);
                        }
                        else
                        {
                            TypesInfo info = new TypesInfo(type);
                            if (info.IsIList)
                            {
                                Type[] types;
                                if (info.IsItList)
                                    types = info.TList;
                                else
                                    types = type.GetGenericArguments();
                                if (types.Length == 1)
                                    elementType = types[0];
                                else
                                    elementType = TType<object>.Type;
                                listType = type;
                                list = (IList)Activator.CreateInstance(type);
                                ReadArray(elementType, list);
                                return list;
                            }
                            else
                            {
                                throw new JsonException("lacks ending quotation before the end of string.", ReadToEnd());
                            }
                        }
                    }
                default: return ReadNumber(type);
            }
        }
        public new T ReadValue()
        {
            object value;
            if (TType<T>.IsPrimitive || TType<Guid>.Type == TType<T>.Type || TType<Money>.Type == TType<T>.Type)
                value = ReadPrimitiveValue(TType<T>.Type);
            else
                value = ReadObjectValue(TType<T>.Type, TAllNameSetFields<T, JsonFieldAttribute>.Fields);
            if (value != null)
                return (T)Convert.ChangeType(value, TType<T>.Type);
            return default(T);
        }
    }
}
