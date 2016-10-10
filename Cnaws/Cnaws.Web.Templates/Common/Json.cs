using Cnaws.Web.Templates.Parser.Node;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Cnaws.Web.Templates.Common
{
    internal sealed class Json
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
        internal sealed class JsonReader : JsonReaderBase
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

            private ArrayTag ReadObject(TemplateContext context)
            {
                Debug.Assert(Peek() == '{');
                Read(); // Skip '{'
                SkipWhitespace();
                ArrayTag value = new ArrayTag();
                if (Peek() == '}')
                {
                    Read();
                    return value;
                }
                StringTag key;
                for (int memberCount = 0; ;)
                {
                    if (Peek() != '"')
                        throw new ParseException(string.Concat("Name of an object member must be a string. ", ReadToEnd()));
                    key = ReadString();
                    SkipWhitespace();
                    if (Peek() != ':')
                        throw new ParseException(string.Concat("There must be a colon after the name of object member. ", ReadToEnd()));
                    Read(); // Skip ':'
                    SkipWhitespace();
                    value.Set(key.ToString(), ReadValue(context), context);
                    SkipWhitespace();
                    ++memberCount;
                    switch (Read())
                    {
                        case ',': SkipWhitespace(); break;
                        case '}': return value;
                        default: throw new ParseException(string.Concat("Must be a comma or '}' after an object member. ", ReadToEnd()));
                    }
                }
            }
            private ArrayTag ReadArray(TemplateContext context)
            {
                Debug.Assert(Peek() == '[');
                Read(); // Skip '['
                SkipWhitespace();
                ArrayTag value = new ArrayTag();
                if (Peek() == ']')
                {
                    Read(); // Skip ']'
                    return value;
                }
                for (;;)
                {
                    value.Set(value.Count, ReadValue(context), context);
                    SkipWhitespace();
                    switch (Read())
                    {
                        case ',': SkipWhitespace(); break;
                        case ']': return value;
                        default: throw new ParseException(string.Concat("Must be a comma or ']' after an array element. ", ReadToEnd()));
                    }
                }
            }
            private NullTag ReadNull()
            {
                Debug.Assert(Peek() == 'n');
                Read();
                if (Read() == 'u' && Read() == 'l' && Read() == 'l')
                    return new NullTag();
                throw new ParseException(string.Concat("Invalid value. ", ReadToEnd()));
            }
            private BooleanTag ReadTrue()
            {
                Debug.Assert(Peek() == 't');
                Read();
                if (Read() == 'r' && Read() == 'u' && Read() == 'e')
                    return new BooleanTag(true);
                throw new ParseException(string.Concat("Invalid value. ", ReadToEnd()));
            }
            private BooleanTag ReadFalse()
            {
                Debug.Assert(Peek() == 'f');
                Read();
                if (Read() == 'a' && Read() == 'l' && Read() == 's' && Read() == 'e')
                    return new BooleanTag(false);
                throw new ParseException(string.Concat("Invalid value. ", ReadToEnd()));
            }
            /// <exception cref="System.ObjectDisposedException"></exception>
            /// <exception cref="System.ArgumentOutOfRangeException"></exception>
            private StringTag ReadString()
            {
                Debug.Assert(Peek() == '\"');
                Read(); // Skip '\"'
                int c, e;
                byte f;
                StringBuilder sb = new StringBuilder();
                for (;;)
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
                                throw new ParseException(string.Concat("Incorrect hex digit after \\u escape. ", ReadToEnd()));
                            try
                            {
                                byte byteAfter = Convert.ToByte(new string(buff, 0, 2), 16);
                                byte byteBefore = Convert.ToByte(new string(buff, 2, 2), 16);
                                sb.Append(Encoding.Unicode.GetString(new byte[] { byteBefore, byteAfter }));
                            }
                            catch
                            {
                                throw new ParseException(string.Concat("Incorrect hex digit after \\u escape. ", ReadToEnd()));
                            }
                        }
                        else
                        {
                            throw new ParseException(string.Concat("Unknown escape character. ", ReadToEnd()));
                        }
                    }
                    else if (c == '"')
                    {
                        return new StringTag(sb.ToString());
                    }
                    else if (c == -1)
                    {
                        throw new ParseException(string.Concat("lacks ending quotation before the end of string. ", ReadToEnd()));
                    }
                    else if (c < 0x20)
                    {
                        throw new ParseException(string.Concat("Incorrect unescaped character in string. ", ReadToEnd()));
                    }
                    else
                    {
                        sb.Append((char)c);
                    }
                }
            }
            private NumberTag ReadNumber()
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
                    return new NumberTag(double.Parse(s, style, NumberFormatInfo.CurrentInfo));
                decimal value = decimal.Parse(s, style, NumberFormatInfo.CurrentInfo);
                if (value >= int.MinValue && value <= int.MaxValue)
                    return new NumberTag((int)value);
                if (value >= long.MinValue && value <= long.MaxValue)
                    return new NumberTag((long)value);
                return new NumberTag(value);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <exception cref="System.ObjectDisposedException"></exception>
            /// <exception cref="System.IO.EndOfStreamException"></exception>
            /// <exception cref="Cnaws.Json.JsonException"></exception>
            /// <returns></returns>
            public Tag ReadValue(TemplateContext context)
            {
                int ch = Peek();
                if (ch == -1)
                    throw new ParseException(string.Concat("lacks ending quotation before the end of string. ", ReadToEnd()));
                switch (ch)
                {
                    case 'n': return ReadNull();
                    case 't': return ReadTrue();
                    case 'f': return ReadFalse();
                    case '"': return ReadString();
                    case '{': return ReadObject(context);
                    case '[': return ReadArray(context);
                    default: return ReadNumber();
                }
            }
        }
    }
}
