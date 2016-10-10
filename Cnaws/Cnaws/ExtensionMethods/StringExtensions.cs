using System;
using System.Text.RegularExpressions;
using Cnaws.Text;
using Cnaws.Security;
using System.Globalization;

namespace Cnaws.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }
        /// <summary> 
        /// 繁体转简体 
        /// </summary> 
        /// <param name="pSource">要转换的繁体字</param> 
        /// <returns>转换后的简体字</returns> 
        public static string ToSimplified(this string s)
        {
            return SystemChineseConverter.ToSimplified(s);
        }
        /// <summary> 
        /// 简体转繁体 
        /// </summary> 
        /// <param name="pSource">要转换的简体字</param> 
        /// <returns>转换后的繁体字</returns> 
        public static string ToTraditional(this string s)
        {
            return SystemChineseConverter.ToTraditional(s);
        }

        public static string MD5(this string s)
        {
            return CryptoUtility.MD5(s);
        }

        public static string TripleDESEncrypt(this string s, string key, string iv)
        {
            return CryptoUtility.TripleDESEncrypt(s, key, iv);
        }
        public static string TripleDESDecrypt(this string s, string key, string iv)
        {
            return CryptoUtility.TripleDESDecrypt(s, key, iv);
        }

        public static bool IsMatch(this string regular, string input)
        {
            if (input == null)
                return false;
            return (new Regex(regular, RegexOptions.Singleline | RegexOptions.IgnoreCase)).IsMatch(input);
        }
        [CLSCompliant(false)]
        public static bool IsMatch(this string regular, ref string input)
        {
            if (input == null)
                return false;
            input = input.Trim();
            return (new Regex(regular, RegexOptions.Singleline | RegexOptions.IgnoreCase)).IsMatch(input);
        }
    }
}
