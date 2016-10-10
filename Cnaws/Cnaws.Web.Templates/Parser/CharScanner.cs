using Cnaws.Web.Templates.Parser.Node;
using System;
using System.Text;

namespace Cnaws.Web.Templates.Parser
{
    /// <summary>
    /// 字符扫描器
    /// </summary>
    public class CharScanner
    {
        /// <summary>
        /// 结束字符
        /// </summary>
        const char EOF = '\0';
        private int index;
        private int start;
        private string document;

        /// <summary>
        /// CharScanner
        /// </summary>
        /// <param name="text">搜描内容</param>
        public CharScanner(string text)
        {
            this.document = text == null ? string.Empty : text;
        }

        /// <summary>
        /// 当前索引
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }
        /// <summary>
        /// 前进1个字符
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            return Next(1);
        }
        /// <summary>
        /// 前进指定介字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public bool Next(int i)
        {
            if (this.index + i > this.document.Length)
                return false;
            this.index += i;
            return true;
        }
        /// <summary>
        /// 后退一个字符
        /// </summary>
        /// <returns></returns>
        public bool Back()
        {
            return Back(1);
        }
        /// <summary>
        /// 后退指定字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public bool Back(int i)
        {
            if (this.index < i)
                return false;
            this.index -= i;
            return true;
        }
        /// <summary>
        /// 读取当前字符
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            return Read(0);
        }
        /// <summary>
        /// 读取当前索引位开始后第i个字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public unsafe char Read(int i)
        {
            if (this.index + i >= this.document.Length)
                return EOF;
            fixed (char* p = this.document)
                return *(p + this.index + i);
        }
        /// <summary>
        /// 当前是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <returns></returns>
        public bool IsMatch(char[] list)
        {
            return IsMatch(list, 0);
        }
        /// <summary>
        /// 是否扫描结束
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return this.index >= this.document.Length;
        }
        /// <summary>
        /// 是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <param name="n">从当前索引后第N位开始</param>
        /// <returns></returns>
        public unsafe bool IsMatch(char[] list, int n)
        {
            n = this.index + n;
            if (this.document.Length >= n + list.Length)
            {
                fixed (char* p = this.document)
                {
                    fixed (char* q = list)
                    {
                        for (int i = 0; i < list.Length; ++i)
                        {
                            if (*(p + n + i) != *(q + i))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 截取start到index的字符串
        /// </summary>
        /// <returns></returns>
        public string GetString(TokenKind tk, char c = char.MinValue)
        {
            string value = GetString(this.start, tk, c);
            this.start = this.index;
            return value;
        }
        /// <summary>
        /// 截取s到index的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <returns></returns>
        public string GetString(int x, TokenKind tk, char c = char.MinValue)
        {
            return GetString(x, this.index, tk, c);
        }
        /// <summary>
        /// 截取x到y的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public unsafe string GetString(int x, int y, TokenKind tk, char c = char.MinValue)
        {
            int len = y - x;
            if (len > 0)
            {
                fixed (char* p = this.document)
                {
                    if (tk == TokenKind.StringEnd)
                    {
                        StringBuilder sb = new StringBuilder(len);
                        char* b = p + x;
                        char* e = b + len;
                        for (char* i = b; i != e; ++i)
                        {
                            if (*i != '\\' || *(i + 1) != c)
                                sb.Append(*i);
                        }

                        return sb.ToString();
                    }
                    return new string(p, x, len);
                }
            }
            return string.Empty;
        }
    }
}
