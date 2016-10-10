using System;
using System.Runtime.InteropServices;

namespace Cnaws.Text
{
    public sealed class SystemChineseConverter
    {
        internal const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        internal const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        internal const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        /// <summary> 
        /// 繁体转简体 
        /// </summary> 
        /// <param name="pSource">要转换的繁体字</param> 
        /// <returns>转换后的简体字</returns> 
        public static string ToSimplified(string pSource)
        {
            string tTarget = new string(' ', pSource.Length);
            int tReturn = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, pSource, pSource.Length, tTarget, pSource.Length);
            return tTarget;
        }

        /// <summary> 
        /// 简体转繁体 
        /// </summary> 
        /// <param name="pSource">要转换的简体字</param> 
        /// <returns>转换后的繁体字</returns> 
        public static string ToTraditional(string pSource)
        {
            string tTarget = new string(' ', pSource.Length);
            int tReturn = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, pSource, pSource.Length, tTarget, pSource.Length);
            return tTarget;
        }
    }
}
