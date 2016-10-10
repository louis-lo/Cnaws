using System;
using System.IO;

namespace Cnaws.ExtensionMethods
{
    public static class DirectoryInfoExtensions
    {
        public static void CreateDirectories(this DirectoryInfo dir)
        {
            if (dir != null && !dir.Exists)
            {
                CreateDirectories(dir.Parent);
                dir.Create();
            }
        }
    }
}
