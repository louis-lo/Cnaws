using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

namespace Cnaws.Web
{
    internal class ModuleInfo
    {
        private string name;
        private string version;

        private ModuleInfo(string n, string v)
        {
            name = n;
            version = v;
        }

        public string Name { get { return name; } }
        public string Version { get { return version; } }

        public static FileInfo[] GetAllFiles(HttpContext context)
        {
            DirectoryInfo dir = new DirectoryInfo(context.Server.MapPath("~/Bin"));
            FileInfo[] files = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            Array.Sort<FileInfo>(files, new FileInfoComparer());
            return files;
        }

        public static List<ModuleInfo> GetAllList(HttpContext context)
        {
            AssemblyName name;
            List<ModuleInfo> list = new List<ModuleInfo>();
            FileInfo[] files = GetAllFiles(context);
            foreach (FileInfo file in files)
            {
                name = Assembly.LoadFrom(file.FullName).GetName();
                list.Add(new ModuleInfo(name.Name, name.Version.ToString()));
            }
            return list;
        }
        public static Dictionary<string, string> GetAllDictionary(HttpContext context)
        {
            AssemblyName name;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            FileInfo[] files = GetAllFiles(context);
            foreach (FileInfo file in files)
            {
                name = Assembly.LoadFrom(file.FullName).GetName();
                dict.Add(name.Name, name.Version.ToString());
            }
            return dict;
        }

        private sealed class FileInfoComparer : IComparer<FileInfo>
        {
            public unsafe int Compare(FileInfo x, FileInfo y)
            {
                int v;
                string sx = x.Name.Substring(0, x.Name.Length - x.Extension.Length);
                string sy = y.Name.Substring(0, y.Name.Length - y.Extension.Length);
                fixed (char* px = sx)
                {
                    char* bx = px;
                    char* ex = px + sx.Length;
                    fixed (char* py = sy)
                    {
                        char* by = py;
                        char* ey = py + sy.Length;
                        for (; bx < ex && by < ey; ++bx, ++by)
                        {
                            v = *bx - *by;
                            if (v != 0)
                                return v;
                        }
                        return sx.Length - sy.Length;
                    }
                }
            }
        }
    }
}
