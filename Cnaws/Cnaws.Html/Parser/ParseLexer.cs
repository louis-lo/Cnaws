using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Html.Parser
{
    internal enum LexerMode
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

    internal sealed class ParseLexer
    {
        private LexerMode _mode;
        private int _column;
        private int _line;
        private TokenKind _kind;
        private int _startColumn;
        private int _startLine;
        private CharScanner _scanner;
        private List<Token> _collection;
        private Stack<string> _pos;

        public unsafe ParseLexer(char* text, int length)
        {
            _mode = LexerMode.None;
            _line = 1;
            _column = 1;
            _kind = TokenKind.Text;
            _startColumn = 1;
            _startLine = 1;
            _scanner = new CharScanner(text, length);
            _collection = new List<Token>();
            _pos = new Stack<string>();
        }

        private Token GetToken(TokenKind tokenKind, char c = char.MinValue)
        {
            Token token = new Token(_kind, _scanner.GetString(tokenKind, c));
            token.BeginLine = _startLine;
            token.BeginColumn = _startColumn;
            token.EndColumn = _column;
            token.EndLine = _line;
            _kind = tokenKind;
            _startColumn = _column;
            _startLine = _line;
            return token;
        }

        private bool Next()
        {
            return Next(1);
        }
        private bool Next(int i)
        {
            if (_scanner.Next(i))
            {
                _column += i;
                return true;
            }
            return false;
        }

        private bool IsElementStart()
        {
            return _kind == TokenKind.Text && _scanner.Read() == '<' && char.IsLetter(_scanner.Read(1));
        }
        private bool IsElementEnd()
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

        public Token[] Parse()
        {
            if (_kind != TokenKind.EOF)
            {
                char c;
                StringBuilder sb;
                do
                {
                    if (_mode == LexerMode.EnterLabel)
                    {
                        Next(1); //skip <
                        sb = new StringBuilder();
                        sb.Append(_scanner.Read());
                        Next(1);
                        while (true)
                        {
                            c = _scanner.Read();
                            if (char.IsLetter(c) || char.IsDigit(c))
                            {
                                sb.Append(c);
                                Next(1);
                            }
                            else
                            {
                                break;
                            }
                        }
                        _pos.Push(sb.ToString());
                        AddToken(GetToken(TokenKind.Element));

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
                    else if (IsElementStart())
                    {
                        AddToken(GetToken(TokenKind.ElementStart));
                        _mode = LexerMode.EnterLabel;
                    }
                    else if (_scanner.Read() == '\n')
                    {
                        ++_line;
                        _column = 1;
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
