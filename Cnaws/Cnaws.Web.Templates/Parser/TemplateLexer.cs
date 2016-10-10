using System;
using System.Collections.Generic;
using Cnaws.Web.Templates.Common;
using Cnaws.Web.Templates.Parser.Node;

namespace Cnaws.Web.Templates.Parser
{
    /// <summary>
    /// 表示词法分析模式的枚举值。
    /// </summary>
    /// <remarks></remarks>
    public enum LexerMode
    {
        /// <summary>
        /// 未定义状态。
        /// </summary>
        None = 0,

        /// <summary>
        /// 进入标签。
        /// </summary>
        EnterLabel,

        /// <summary>
        /// 脱离标签。
        /// </summary>
        LeaveLabel,

    }

    /// <summary>
    /// 词素分析器
    /// </summary>
    public class TemplateLexer
    {
        /// <summary>
        /// 状态
        /// </summary>
        private LexerMode mode;
        /// <summary>
        /// 当前文档
        /// </summary>
        private string document;
        /// <summary>
        /// 当前列
        /// </summary>
        private int column;
        /// <summary>
        /// 当前行
        /// </summary>
        private int line;
        /// <summary>
        /// 当前TokenKind
        /// </summary>
        private TokenKind kind;
        /// <summary>
        /// 起始列
        /// </summary>
        private int startColumn;
        /// <summary>
        /// 起始行
        /// </summary>
        private int startLine;
        /// <summary>
        /// 扫描器
        /// </summary>
        private CharScanner scanner;

        private List<Token> collection;

        private Stack<string> pos;

        /// <summary>
        /// TemplateLexer
        /// </summary>
        /// <param name="text">文本内容</param>
        public TemplateLexer(string text)
        {
            this.document = text;
            Reset();
        }
        /// <summary>
        /// 重置分析器
        /// </summary>
        public void Reset()
        {
            this.mode = LexerMode.None;
            this.line = 1;
            this.column = 1;
            this.kind = TokenKind.Text;
            this.startColumn = 1;
            this.startLine = 1;
            this.scanner = new CharScanner(this.document);
            this.collection = new List<Token>();
            this.pos = new Stack<string>();
        }

        private Token GetToken(TokenKind tokenKind, char c = char.MinValue)
        {
            Token _token = new Token(this.kind, this.scanner.GetString(tokenKind, c));
            _token.BeginLine = this.startLine;
            _token.BeginColumn = this.startColumn;
            _token.EndColumn = this.column;
            _token.EndLine = this.line;
            this.kind = tokenKind;
            this.startColumn = this.column;
            this.startLine = this.line;
            return _token;
        }

        private bool Next()
        {
            return Next(1);
        }
        private bool Next(int i)
        {
            if (this.scanner.Next(i))
            {
                this.column += i;
                return true;
            }

            return false;
        }

        private bool IsTagStart()
        {
            if (this.scanner.Read() == '$')
            {
                if (!this.scanner.IsEnd())
                {
                    char value = this.scanner.Read(1);
                    if (value == '{')
                    {
                        this.pos.Push("${");
                        return true;
                    }
                    if (char.IsLetter(value))
                    {
                        this.pos.Push("$");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsTagEnd()
        {
            if (this.pos.Count == 1)
            {
                if (this.scanner.IsEnd())
                {
                    return true;
                }
                char value = this.scanner.Read();
                if (this.pos.Peek().Length == 2)
                {
                    if (value == '}')
                    {
                        return true;
                    }
                }
                else if (value != '.')
                {
                    if (((value == '(' || ParserHelpers.IsWord(value)) && ParserHelpers.IsWord(this.scanner.Read(-1)))
                        || ((value == '[' || ParserHelpers.IsWord(value)) && ParserHelpers.IsWord(this.scanner.Read(-1)))
                        || (ParserHelpers.IsWord(value) && (this.scanner.Read(-1) == '.')))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 分析所有Token
        /// </summary>
        /// <returns></returns>
        public Token[] Parse()
        {
            if (this.kind != TokenKind.EOF)
            {
                char c;
                do
                {
                    if (this.mode == LexerMode.EnterLabel)
                    {
                        Next(this.pos.Peek().Length - 1);
                        c = this.scanner.Read();
                        AddToken(GetToken(GetTokenKind(c), c));
                        switch (this.kind)
                        {
                            case TokenKind.StringStart:
                                this.pos.Push(c.ToString());
                                break;
                            case TokenKind.LeftParentheses:
                                this.pos.Push("(");
                                break;
                            case TokenKind.LeftBracket:
                                this.pos.Push("[");
                                break;
                        }
                        ReadToken();
                    }
                    else if (IsTagStart())
                    {
                        AddToken(GetToken(TokenKind.TagStart));
                        this.mode = LexerMode.EnterLabel;

                    }
                    else if (this.scanner.Read() == '\n')
                    {
                        this.line++;
                        this.column = 1;
                    }
                }
                while (Next());

                AddToken(GetToken(TokenKind.EOF));


                if (this.mode == LexerMode.EnterLabel)
                {
                    this.mode = LexerMode.LeaveLabel;
                    AddToken(new Token(TokenKind.TagEnd, String.Empty));
                }

            }

            return this.collection.ToArray();

        }

        private void AddToken(Token token)
        {
            if (this.collection.Count > 0 && this.collection[this.collection.Count - 1].Next == null)
            {
                this.collection[this.collection.Count - 1].Next = token;
            }
            this.collection.Add(token);
        }

        private void ReadToken()
        {
            char c;
            string s;
            while (Next())
            {
                c = this.scanner.Read();
                s = c.ToString();
                if (c == '"' || c == '\'')
                {
                    if (this.pos.Count > 1)
                    {
                        if (this.pos.Peek() == s)
                        {
                            if (this.kind == TokenKind.StringStart)
                            {
                                AddToken(GetToken(TokenKind.String));
                            }
                            if (this.scanner.Read(-1) != '\\')
                            {
                                AddToken(GetToken(TokenKind.StringEnd, c));
                                this.pos.Pop();
                            }
                            continue;
                        }
                        else
                        {
                            if (this.kind == TokenKind.StringStart)
                            {
                                AddToken(GetToken(TokenKind.String));
                                continue;
                            }
                        }
                    }

                    if (this.kind == TokenKind.TagStart
                        || this.kind == TokenKind.LeftBracket
                        || this.kind == TokenKind.LeftParentheses
                        || this.kind == TokenKind.Operator
                        || this.kind == TokenKind.Punctuation
                        || this.kind == TokenKind.Comma
                        || this.kind == TokenKind.Space)
                    {
                        AddToken(GetToken(TokenKind.StringStart));
                        this.pos.Push(s);
                        continue;
                    }
                }

                if (this.kind == TokenKind.StringStart)
                {
                    AddToken(GetToken(TokenKind.String));
                    continue;
                }

                if (this.kind == TokenKind.String)
                {
                    continue;
                }

                if (this.scanner.Read() == '(')
                {
                    this.pos.Push("(");
                }
                else if (this.scanner.Read() == '[')
                {
                    this.pos.Push("[");
                }
                else if (this.scanner.Read() == ')' && this.pos.Peek() == "(")// && this.pos.Count > 2
                {
                    this.pos.Pop();
                    if (this.pos.Count == 1)
                    {

                    }
                }
                else if (this.scanner.Read() == ']' && this.pos.Peek() == "[")// && this.pos.Count > 2
                {
                    this.pos.Pop();
                    if (this.pos.Count == 1)
                    {

                    }
                }
                else if (IsTagEnd())
                {
                    //Next(1);
                    //this.pos.Pop();
                    AddToken(GetToken(TokenKind.TagEnd));
                    this.mode = LexerMode.LeaveLabel;
                    if (this.pos.Pop().Length == 2)
                    {
                        Next(1);
                    }
                    if (IsTagStart())
                    {
                        AddToken(GetToken(TokenKind.TagStart));
                        this.mode = LexerMode.EnterLabel;
                    }
                    else
                    {
                        AddToken(GetToken(TokenKind.Text));
                    }
                    break;
                }
                TokenKind tk;
                char tc = char.MinValue;
                if (this.scanner.Read() == '+' || this.scanner.Read() == '-') //正负数符号识别
                {
                    if (Char.IsNumber(this.scanner.Read(1)) &&
                        (this.kind == TokenKind.Operator || this.kind == TokenKind.LeftParentheses))
                    {
                        tk = TokenKind.Number;
                    }
                    else
                    {
                        tk = TokenKind.Operator;
                    }
                }
                else
                {
                    tc = this.scanner.Read();
                    tk = GetTokenKind(tc);
                }
                //if (this.kind == tk || (tk == TokenKind.Number && this.kind == TokenKind.TextData))
                if ((this.kind != tk || this.kind == TokenKind.LeftParentheses || this.kind == TokenKind.RightParentheses)
                    && (tk != TokenKind.Number || this.kind != TokenKind.TextData)
                    //&& (this.kind == TokenKind.Number && tk != TokenKind.Dot)
                    )
                //|| (this.kind != TokenKind.Number && tk == TokenKind.Dot)
                {
                    if (tk == TokenKind.Dot && this.kind == TokenKind.Number)
                    {

                    }
                    else
                    {
                        AddToken(GetToken(tk, tc));
                    }
                }

            }
        }

        private TokenKind GetTokenKind(char c)
        {
            if (this.mode != LexerMode.EnterLabel)
            {
                return TokenKind.Text;
            }
            switch (c)
            {
                case ' ':
                    return TokenKind.Space;
                case '(':
                    return TokenKind.LeftParentheses;
                case ')':
                    return TokenKind.RightParentheses;
                case '[':
                    return TokenKind.LeftBracket;
                case ']':
                    return TokenKind.RightBracket;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return TokenKind.Number;
                case '*':
                case '-':
                case '+':
                case '/':
                case '>':
                case '<':
                case '=':
                case '!':
                case '&':
                case '|':
                case '~':
                case '^':
                case '?':
                case '%':
                    return TokenKind.Operator;
                case ',':
                    return TokenKind.Comma;
                case '.':
                    return TokenKind.Dot;
                case '"':
                case '\'':
                    return TokenKind.StringStart;
                case ';':
                    return TokenKind.Punctuation;
                default:
                    return TokenKind.TextData;
            }
        }

    }
}
