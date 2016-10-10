using System;
using System.IO;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using Cnaws.ExtensionMethods;

namespace Cnaws.Web.Caching
{
    internal sealed class FileCache : CacheProvider
    {
        public static readonly FileCache Instance;

        static FileCache()
        {
            Instance = new FileCache();
        }
        private FileCache()
        {
        }

        protected override string FormatKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return Path.Combine(HttpContext.Current.Server.MapPath(Utility.CacheDir), key);
        }
        protected override string FormatKeys(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            string[] array = new string[keys.Length + 1];
            array[0] = HttpContext.Current.Server.MapPath(Utility.CacheDir);
            Array.Copy(keys, 0, array, 1, keys.Length);
            return Path.Combine(array);
        }
        private string GetKey(string path)
        {
            return path.Substring(HttpContext.Current.Server.MapPath(Utility.CacheDir).Length).ToUpper().Replace(Path.DirectorySeparatorChar, '.');
        }
        protected override object GetImpl(string key)
        {
            try
            {
                using (FileStream ms = new FileStream(key, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(ms);
                }
            }
            catch (Exception
#if(DEBUG)
            ex
#endif
            )
            { }
            return null;
        }
        protected override void SetImpl(string key, object value)
        {
            try
            {
                FileInfo file = new FileInfo(key);
                file.Directory.CreateDirectories();

                using (FileStream ms = new FileStream(key, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    format.Serialize(ms, value);
                }
            }
            catch (Exception
#if (DEBUG)
            ex
#endif
            )
            { }
        }
        protected override void DeleteImpl(string key)
        {
            if (File.Exists(key))
            {
                try { File.Delete(key); }
                catch (Exception) { }
            }
            if (Directory.Exists(key))
            {
                try { Directory.Delete(key, true); }
                catch (Exception) { }
            }
        }
        public override void Clear()
        {
            try { Directory.Delete(HttpContext.Current.Server.MapPath(Utility.CacheDir), true); }
            catch (Exception) { }
        }
    }
}
