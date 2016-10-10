using System;

namespace Cnaws.Web.Templates.Common
{
    /// <summary>
    /// 分析辅助类
    /// </summary>
    internal static class ParserHelpers
    {
        /// <summary>
        /// 是否英文字母
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool IsLetter(char value)
        {
            return char.IsLower(value) || char.IsUpper(value);
        }
        /// <summary>
        /// 是否单词
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool IsWord(char value)
        {
            return char.IsLower(value) || char.IsUpper(value) || char.IsNumber(value) || value == '_';
        }
        /// <summary>
        /// 字符串是否相同
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsEqual(string x, string y)
        {
            if (x == null || y == null)
                return x == y;
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
