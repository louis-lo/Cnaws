using System;
using System.Collections.Generic;
using Cnaws.Web.Templates.Parser.Node;

namespace Cnaws.Web.Templates.Parser
{
    /// <summary>
    /// TemplateParser
    /// </summary>
    public class TemplateParser : IEnumerator<Tag>
    {
        private const StringComparison stringComparer = StringComparison.OrdinalIgnoreCase;

        #region private field
        private TemplateContext context;
        private Tag tag;//当前标签
        private Token[] tokens;//tokens列表
        private int index;//当前索引
        private static List<ITagParser> parsers;
        #endregion

        #region ctox
        public TemplateParser(TemplateContext context, Token[] ts)
        {
            parsers = new List<ITagParser>();
            this.context = context;
            this.tokens = ts;
            Reset();
        }
        #endregion

        public TemplateContext Context
        {
            get { return context; }
        }

        #region IEnumerator<Tag> 成员
        /// <summary>
        /// 当前标签
        /// </summary>
        public Tag Current
        {
            get { return tag; }
        }

        #endregion

        #region IEnumerator 成员

        /// <summary>
        /// 读取下一个标签
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (this.index < this.tokens.Length)
            {
                Tag t = Read();
                if (t != null)
                {
                    this.tag = t;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            this.index = 0;
            this.tag = null;
        }

        private Tag Read()
        {
            Tag t = null;
            if (IsTagStart())
            {
                Token t1, t2;
                t1 = t2 = GetToken();
                TokenCollection tc = new TokenCollection();

                do
                {
                    ++this.index;
                    t2.Next = GetToken();
                    t2 = t2.Next;

                    tc.Add(t2);


                } while (!IsTagEnd());

                tc.Remove(tc.Last);

                ++this.index;

                try
                {
                    t = Read(tc);
                }
                catch (TemplateException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new ParseException(string.Concat("Parse error:", tc, "\r\nError message:", e.Message), context.CurrentPath, tc.First.BeginLine, tc.First.BeginColumn);//标签分析异常
                }

                if (t != null)
                {
                    t.FirstToken = t1;
                    if (t.Children.Count == 0 || t.LastToken == null || t2.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = t2;
                    }
                }
                else
                {
                    throw new ParseException(string.Concat("Unexpected  tag:", tc), context.CurrentPath, tc.First.BeginLine, tc.First.BeginColumn); //未知的标签
                }
            }
            else
            {
                t = new TextTag();
                t.FirstToken = GetToken();
                t.LastToken = null;
                ++this.index;
            }
            return t;
        }

        /// <summary>
        /// 读取一个标签
        /// </summary>
        /// <param name="tc">TOKEN集合</param>
        /// <returns></returns>
        public Tag Read(TokenCollection tc)
        {
            if (tc == null || tc.Count == 0)
                throw new ParseException("Invalid TokenCollection!");//无效的标签集合
            return Parser.Parse(this, tc);
        }

        private bool IsTagEnd()
        {
            return IsTagEnd(GetToken());
        }

        private bool IsTagStart()
        {
            return IsTagStart(GetToken());
        }

        private bool IsTagEnd(Token t)
        {
            return t == null || t.TokenKind == TokenKind.TagEnd || t.TokenKind == TokenKind.EOF;
        }

        private bool IsTagStart(Token t)
        {
            return t.TokenKind == TokenKind.TagStart;
        }

        private Token GetToken()
        {
            return tokens[this.index];
        }

        #endregion

        #region IEnumerator 成员

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        #endregion

        #region IDispose 成员
        public void Dispose()
        {
            parsers.Clear();
        }
        #endregion
    }
}
