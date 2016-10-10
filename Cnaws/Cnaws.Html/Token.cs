using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Html
{
    internal sealed class Token : IComparable<Token>
    {
        private string _text;
        private int _beginLine;
        private int _beginColumn;
        private int _endLine;
        private int _endColumn;
        private TokenKind _tokenKind;
        private Token _next;

        public Token(TokenKind kind, string text)
        {
            _tokenKind = kind;
            _text = text;
        }

        public int BeginLine
        {
            get { return _beginLine; }
            set { _beginLine = value; }
        }
        public int BeginColumn
        {
            get { return _beginColumn; }
            set { _beginColumn = value; }
        }
        public int EndLine
        {
            get { return _endLine; }
            set { _endLine = value; }
        }
        public int EndColumn
        {
            get { return _endColumn; }
            set { _endColumn = value; }
        }
        public string Text
        {
            get { return _text; }
        }
        public TokenKind TokenKind
        {
            get { return _tokenKind; }
        }
        public Token Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public override string ToString()
        {
            return Text;
        }

        public int CompareTo(Token other)
        {
            if (BeginLine > other.BeginLine)
                return 1;
            if (BeginLine < other.BeginLine)
                return -1;
            if (BeginColumn > other.BeginColumn)
                return 1;
            if (BeginColumn < other.BeginColumn)
                return -1;
            return 0;
        }
    }
}
