using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Cnaws.Security
{
    public static class CryptoUtility
    {
        public static string MD5(byte[] bytes)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] s = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < s.Length; i++) sb.Append(s[i].ToString("x2"));
                return sb.ToString();
            }
        }
        public static string MD5(string s)
        {
            return MD5(Encoding.UTF8.GetBytes(s));
        }

        public static string TripleDESEncrypt(byte[] bytes, byte[] key, byte[] iv)
        {
            using (TripleDES tdes = TripleDESCryptoServiceProvider.Create())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ICryptoTransform ictf = tdes.CreateEncryptor(key, iv))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, ictf, CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.FlushFinalBlock();
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        public static byte[] TripleDESDecrypt(string s, byte[] key, byte[] iv)
        {
            using (TripleDES tdes = TripleDESCryptoServiceProvider.Create())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ICryptoTransform ictf = tdes.CreateDecryptor(key, iv))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, ictf, CryptoStreamMode.Write))
                        {
                            byte[] bytes = Convert.FromBase64String(s);
                            cs.Write(bytes, 0, bytes.Length);
                            cs.FlushFinalBlock();
                        }
                    }
                    return ms.ToArray();
                }
            }
        }
        public static string TripleDESEncrypt(string s, string key, string iv)
        {
            return TripleDESEncrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static string TripleDESDecrypt(string s, string key, string iv)
        {
            return Encoding.UTF8.GetString(TripleDESDecrypt(s, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv)));
        }
    }
}
