using System;
using System.Text;

namespace Cnaws.Html
{
    internal unsafe sealed class StringBuffer
    {
        private static readonly bool[] CHAR_SCAPE;

        private char* _begin;
        private char* _end;
        private char* _current;

        static StringBuffer()
        {
            CHAR_SCAPE = new bool[byte.MaxValue + 1];
            CHAR_SCAPE[' '] = true;
            CHAR_SCAPE['\t'] = true;
            CHAR_SCAPE['\r'] = true;
            CHAR_SCAPE['\n'] = true;
        }
        public StringBuffer(char* pstr, int length)
        {
            _begin = pstr;
            _end = _begin + length;
            _current = _begin;
        }

        public bool IsEnd
        {
            get { return _current >= _end; }
        }

        public char Peek()
        {
            return *(_current + 1);
        }
        public char Read()
        {
            return *(++_current);
        }

        public void SkipWhiteSpace()
        {
            while (_current < _end)
            {
                if (CHAR_SCAPE[*_current])
                    ++_current;
                else
                    break;
            }
        }
    }
}
