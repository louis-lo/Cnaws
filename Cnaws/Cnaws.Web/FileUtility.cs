using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Web;

namespace Cnaws.Web
{
    internal static class FileUtility
    {
        public static void WriteFile(HttpResponse response, string path)
        {
            MemoryMappedFile file = null;
            try
            {
                try { file = MemoryMappedFile.CreateFromFile(path, FileMode.Open, path, 0, MemoryMappedFileAccess.Read); }
                catch (Exception) { }
                if (file == null)
                {
                    try { file = MemoryMappedFile.OpenExisting(path, MemoryMappedFileRights.Read); }
                    catch (Exception) { }
                }
                if (file != null)
                {
                    using (MemoryMappedViewStream stream = file.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
                    {
                        response.w
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                    file = null;
                }
            }
        }
    }
}
