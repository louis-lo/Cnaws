using System;
using System.IO;
using System.Reflection;

namespace ApiHelper
{
    internal sealed class ApiContext : IDisposable
    {
        private AppDomain _domain;
        private bool disposed = false;

        public ApiContext(string path)
        {
            FileInfo file = new FileInfo(path);
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = file.Directory.FullName;
            _domain = AppDomain.CreateDomain("ApiHelper", null, setup);
            string ns = string.Concat(file.Name.Substring(0, file.Name.Length - file.Extension.Length + 1), "Controllers");
            Assembly asm = Assembly.LoadFrom(file.FullName);
            _domain.Load(asm.GetName());
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                if (type.IsClass && type.IsPublic && !type.IsAbstract && string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase))
                {
                    MethodInfo[] methods = type.UnderlyingSystemType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo method in methods)
                    {
                        if (method.Name.Length > 9 && method.Name.StartsWith("Api") && method.Name.EndsWith("Helper"))
                            method.Invoke(null, null);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_domain != null)
                    {
                        AppDomain.Unload(_domain);
                        _domain = null;
                    }
                }
                disposed = true;
            }
        }
        ~ApiContext()
        {
            Dispose(false);
        }
    }
}
