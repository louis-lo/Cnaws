using System;

namespace Cnaws.ExtensionMethods
{
    public static class LongExtensions
    {
        public static bool IsMobile(this long mobile)
        {
            return (mobile >= 13000000000 && mobile < 19000000000);
        }
    }
}
