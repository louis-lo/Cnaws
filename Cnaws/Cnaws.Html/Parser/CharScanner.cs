using System;
using System.Text;

namespace Cnaws.Html.Parser
{
    internal unsafe sealed class CharScanner
    {
        private char* _start;
        private char* _end;
        private char* _current;

        public CharScanner(char* text, int lenght)
        {
            _start = text;
            _end = text + lenght;
            _current = _start;
        }

        public bool Next()
        {
            return Next(1);
        }
        public bool Next(int i)
        {
            if ((_current + i) > _end)
                return false;
            _current += i;
            return true;
        }

        public char Read()
        {
            return Read(0);
        }
        public char Read(int i)
        {
            char* value = _current + i;
            if (value >= _end)
                return char.MinValue;
            return *value;
        }

        public bool IsEnd()
        {
            return _current >= _end;
        }

        public string GetString(TokenKind tk, char c = char.MinValue)
        {
            switch (tk)
            {
                case TokenKind.ElementStart:
                    break;
                case TokenKind.Element:
                    break;
            }
            return string.Empty;
        }

        //public string GetString(TokenKind tk, char c = char.MinValue)
        //{
        //    string value = GetStringImpl(tk, c);
        //    _start = _current;
        //    return value;
        //}
        //private string GetStringImpl(TokenKind tk, char c = char.MinValue)
        //{
        //    int len = (int)(_current - _start);
        //    if (len > 0)
        //    {
        //        if (tk == TokenKind.StringEnd)
        //        {
        //            StringBuilder sb = new StringBuilder(len);
        //            for (char* i = _start; i != _current; ++i)
        //            {
        //                if (*i != '\\' || *(i + 1) != c)
        //                    sb.Append(*i);
        //            }
        //            return sb.ToString();
        //        }
        //        return new string(_start, 0, len);
        //    }
        //    return string.Empty;
        //}
    }
}
