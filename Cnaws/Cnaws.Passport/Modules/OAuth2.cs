using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Passport.OAuth2;
using System.Reflection;
using Cnaws.Templates;
using System.Collections.Generic;

namespace Cnaws.Passport.Modules
{
    [Serializable]
    public sealed class OAuth2 : NoIdentityModule
    {
        [DataColumn(true, 32)]
        public string Id = null;
        [DataColumn(32)]
        public string Name = null;
        [DataColumn(32)]
        public string Version = null;
        [DataColumn(64)]
        public string Key = null;
        [DataColumn(64)]
        public string Secret = null;
        public bool Enabled = false;

        private static string[] GetCacheName(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                return new string[] { "OAuth2", "Module" };
            return new string[] { "OAuth2", "Module", id.ToLower() };
        }

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Enabled");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Enabled", "Enabled");

            OAuth2Provider provider;
            string ns = string.Concat("Cnaws.Passport.OAuth2.Providers");
            Assembly asm = Assembly.GetAssembly(TType<OAuth2>.Type);
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                if (type.IsClass &&
                    !type.IsAbstract &&
                    string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase) &&
                    TType<OAuth2Provider>.Type.IsAssignableFrom(type.UnderlyingSystemType))
                {
                    provider = (OAuth2Provider)Activator.CreateInstance(type.UnderlyingSystemType, new object[] { new OAuth2ProviderOptions() { ClientId = Guid.NewGuid().ToString("N") } });
                    (new OAuth2() { Id = provider.Key, Name = provider.Name, Version = provider.Version.ToString() }).Insert(ds);
                }
            }
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Version))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "Id", "Name", "Version");
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Id))
                return DataStatus.Failed;
            Id = Id.ToLower();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            CacheProvider.Current.Set(GetCacheName(Id), null);
            return DataStatus.Success;
        }

        public static OAuth2 GetById(DataSource ds, string id)
        {
            if (id == null)
                return null;
            id = id.ToLower();
            string[] key = GetCacheName(id);
            OAuth2 result = CacheProvider.Current.Get<OAuth2>(key);
            if (result == null)
            {
                result = ExecuteSingleRow<OAuth2>(ds, P("Id", id));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }

        public static IList<OAuth2> GetAll(DataSource ds)
        {
            string[] key = GetCacheName();
            IList<OAuth2> result = CacheProvider.Current.Get<IList<OAuth2>>(key);
            if (result == null)
            {
                result = ExecuteReader<OAuth2>(ds);
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static SplitPageData<OAuth2> GetPage(DataSource ds, int index, int size, int show = 8)
        {
            int count;
            IList<OAuth2> list = ExecuteReader<OAuth2>(ds, Os(Oa("Id")), index, size, out count);
            return new SplitPageData<OAuth2>(index, size, list, count, show);
        }
    }
}
