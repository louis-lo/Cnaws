using System;

namespace Cnaws.Html
{
    internal static class HtmlUtil
    {
        //9 '\t' 10 '\n' 13 '\r' 32 ' '
        private static readonly bool[] CHAR_SCAPE = new bool[]
        {
            false,false,false,false,false,false,false,false,false,true,true,false,false,true,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false
        };

        //65-90(A-Z) 97-122(a-z)
        private static readonly bool[] CHAR_CHAR = new bool[]
        {
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//15
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//31
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//47
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//63
            false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,//79
            true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,//95
            false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,//111
            true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,//127
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false
        };
        //65-90(A-Z) 97-122(a-z) 48-57(0-9)
        private static readonly bool[] CHAR_CHARANDNUMBER = new bool[]
        {
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//15
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//31
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,//47
            true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,false,//63
            false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,//79
            true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,//95
            false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,//111
            true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,//127
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false
        };

        public static bool IsSpace(char c)
        {
            return CHAR_SCAPE[c];
        }
        public static bool IsChar(char c)
        {
            return CHAR_CHAR[c];
        }
        public static bool IsCharOrNumber(char c)
        {
            return CHAR_CHARANDNUMBER[c];
        }
    }
}
