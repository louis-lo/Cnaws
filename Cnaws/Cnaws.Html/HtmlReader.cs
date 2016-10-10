using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Cnaws.Html
{
    public enum HtmlNodeType
    {
        Comment,
        Element,
        Text,
    }

    internal enum HtmlParseType
    {

    }

    internal sealed class HtmlToken
    {

    }

    internal unsafe sealed class HtmlReader
    {
        private char* _begin;
        private char* _end;
        private char* _current;

        public HtmlReader(char* pstr, int length)
        {
            _begin = pstr;
            _end = _begin + length;
            _current = _begin;
        }

        public bool IsEnd
        {
            get { return _current >= _end; }
        }
        public char Current
        {
            get { return *_current; }
        }
        public char this[int index]
        {
            get { return *(_current + index); }
        }
        
        public char Peek()
        {
            return *(_current + 1);
        }
        public char Read()
        {
            char c = *_current;
            ++_current;
            return c;
        }

        public void SkipWhiteSpace()
        {
            while (_current < _end)
            {
                if (HtmlUtil.IsSpace(*_current))
                    ++_current;
                else
                    break;
            }
        }
    }
}
