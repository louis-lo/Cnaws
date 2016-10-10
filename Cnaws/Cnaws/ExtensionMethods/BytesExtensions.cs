using System;

using Cnaws.Security;

namespace Cnaws.ExtensionMethods
{
    public static class BytesExtensions
    {
        public static string MD5(this byte[] bytes)
        {
            return CryptoUtility.MD5(bytes);
        }
    }
}
