using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cnaws.Web.Templates
{
    /// <summary>
    ///资源操作
    /// </summary>
    public class Resources
    {
        private readonly static Collection<string> collection = new Collection<string>();

        /// <summary>
        /// 资源路径
        /// </summary>
        public static Collection<string> Paths
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="fullPath">查找结果</param>
        /// <returns></returns>
        public static int FindPath(string filename, out string fullPath)
        {
            return FindPath(Paths, filename, out fullPath);
        }
        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns></returns>
        private static int FindPath(IEnumerable<string> paths, string filename, out string fullPath)
        {
            //filename 允许单纯的文件名或相对路径
            fullPath = null;

            if (!string.IsNullOrEmpty(filename))
            {
                filename = NormalizePath(filename);

                if (filename == null) //路径非法，比如用户试图跳出当前目录时（../header.txt）
                {
                    return -1;
                }

                if (filename.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename.Substring(1));
                    return 0;
                }

                //String sc = String.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);
                int i = 0;
                foreach (string checkUrl in paths)
                {
                    //if (checkUrl[checkUrl.Length - 1] != Path.DirectorySeparatorChar)
                    //    fullPath = string.Concat(checkUrl, filename);
                    //else
                    //    fullPath = string.Concat(checkUrl.Remove(checkUrl.Length - 1, 1), filename);
                    fullPath = Path.Combine(checkUrl, filename);
                    if (File.Exists(fullPath))
                        return i;
                    ++i;
                }

            }
            return -1;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string LoadResource(string filename, Encoding encoding)
        {
            return LoadResource(Paths, filename, encoding);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string LoadResource(IEnumerable<string> paths, string filename, Encoding encoding)
        {
            string full;
            if (FindPath(paths, filename, out full) != -1)
            {
                return Load(full, encoding);
            }
            return null;
        }

        /// <summary>
        /// 载入文件
        /// </summary>
        /// <param name="filename">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Load(string filename, Encoding encoding)
        {
            return File.ReadAllText(filename, encoding);
        }

        /// <summary>
        /// 路径处理
        /// </summary>
        /// <param name="filename">如果有目录</param>
        /// <returns></returns>
        public static string NormalizePath(string filename)
        {
            if (string.IsNullOrEmpty(filename) || filename.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                return null;

            //List<string> values = new List<string>(filename.Split('/'));

            //for (int i = 0; i < values.Count; ++i)
            //{
            //    if (values[i] == "." || string.IsNullOrEmpty(values[i]))
            //    {
            //        values.RemoveAt(i);
            //        --i;
            //    }
            //    else if (values[i] == "..")
            //    {
            //        if (i == 0)
            //        {
            //            return null;
            //        }
            //        values.RemoveAt(i);
            //        --i;
            //        values.RemoveAt(i);
            //        --i;
            //    }
            //}

            //values.Insert(0, string.Empty);

            //return string.Join(Path.DirectorySeparatorChar.ToString(), values.ToArray());

            return filename.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
