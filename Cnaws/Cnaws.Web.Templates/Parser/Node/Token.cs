using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    /// <summary>
    /// Token种类
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 文本数据
        /// </summary>
        TextData,
        /// <summary>
        /// 标签
        /// </summary>
        Tag,
        /// <summary>
        /// 标签开始标记
        /// </summary>
        TagStart,
        /// <summary>
        /// 标签结束标记
        /// </summary>
        TagEnd,
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// 左中括号
        /// </summary>
        LeftBracket,
        /// <summary>
        /// 右中括号
        /// </summary>
        RightBracket,
        /// <summary>
        /// 左圆括号
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// 右员括号
        /// </summary>
        RightParentheses,
        /// <summary>
        /// 新行（换行符）
        /// </summary>
        NewLine,
        /// <summary>
        /// 点
        /// </summary>
        Dot,
        /// <summary>
        /// 字符串开始
        /// </summary>
        StringStart,
        /// <summary>
        /// 字符串结束
        /// </summary>
        StringEnd,
        /// <summary>
        /// 空格
        /// </summary>
        Space,
        /// <summary>
        /// 标点
        /// </summary>
        Punctuation,
        /// <summary>
        /// 运算符
        /// </summary>
        Operator,
        /// <summary>
        /// 逗号
        /// </summary>
        Comma,
        /// <summary>
        /// 结束
        /// </summary>
        EOF
    }

    public class Token : IComparable<Token>
    {
        private string _text;
        private int _beginline;
        private int _begincolumn;
        private int _endline;
        private int _endcolumn;
        private TokenKind _tokenkind;
        private Token _next;

        public int BeginLine
        {
            get { return _beginline; }
            set { _beginline = value; }
        }

        public int BeginColumn
        {
            get { return _begincolumn; }
            set { _begincolumn = value; }
        }

        public int EndLine
        {
            get { return _endline; }
            set { _endline = value; }
        }

        public int EndColumn
        {
            get { return _endcolumn; }
            set { _endcolumn = value; }
        }

        public string Text
        {
            get { return _text; }
        }

        public TokenKind TokenKind
        {
            get { return _tokenkind; }
        }

        public Token(TokenKind kind, string text)
        {
            this._tokenkind = kind;
            this._text = text;
        }

        public Token Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public override string ToString()
        {
            return this.Text;
        }

        #region IComparable<Token> 成员

        public int CompareTo(Token other)
        {
            if (this.BeginLine > other.BeginLine)
                return 1;
            if (this.BeginLine < other.BeginLine)
                return -1;
            if (this.BeginColumn > other.BeginColumn)
                return 1;
            if (this.BeginColumn < other.BeginColumn)
                return -1;
            return 0;
        }

        #endregion
    }
}
